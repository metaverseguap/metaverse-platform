using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Global.AssetPackages;
using Global.Converters;
using Global.Logger;
using Localization;
using MainMenu.Containers;
using MainMenu.Containers.Utils;
using Mirror;
using NetworkCore.MirrorNetworking;
using NetworkCore.MirrorNetworking.Containers;
using NetworkCore.ServerInteraction.API;
using NetworkCore.ServerInteraction.Type.Avatar;
using NetworkCore.ServerInteraction.Type.Scene;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu.UI.LoadingScene
{
    /// <summary>
    /// <para>Скрипт загружающий компоненты с файлового сервера.</para>
    /// </summary>
    public sealed class UILoadingMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] [Scene] private string _selectAvatarScene;

        private APIContainer serverAPI;
        private AvatarStore avatarStore;
        private SceneStore sceneStore;
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            MVNetworkManager networkManager = MVNetworkManager.singleton;
            avatarStore = networkManager.NetworkStore.Avatars;
            sceneStore = networkManager.NetworkStore.Scenes;
            serverAPI = networkManager.NetworkStore.FileServer;

            DownloadAssets();
        }

        private async void DownloadAssets()
        {
            await AvatarDownloading();
            await SceneDownloading();
            SceneManager.LoadScene(_selectAvatarScene, LoadSceneMode.Single);
        }

        private async Task AvatarDownloading()
        {
            _loadingText.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading.avatars");

            IList<AvatarInfo> serverAvatarsInfos = serverAPI.Avatar.GetAllAvatarsInfo();
            IList<AvatarInfo> localAvatars = AvatarAssetPackages.GetMatchingLocalAvatars(serverAvatarsInfos);
            serverAvatarsInfos = ContainerUtils.RemoveMatchingElements(serverAvatarsInfos, localAvatars);

            IList<AvatarInfo> newAvatars = await serverAPI.Avatar.GetAvatarsFiles(serverAvatarsInfos);

            SaveAvatarsInfos(localAvatars, newAvatars);
            SaveAvatars(localAvatars, newAvatars);
        }

        private void SaveAvatarsInfos(IList<AvatarInfo> localAvatars, IList<AvatarInfo> newAvatars)
        {
            IList<AvatarInfoDTO> currentAvatarsInfos =
                new List<AvatarInfo>(localAvatars)
                    .Concat(newAvatars)
                    .Select(ToAvatarRO)
                    .ToList();

            AvatarAssetPackages.SaveAvatarInfo(currentAvatarsInfos);
        }

        private static AvatarInfoDTO ToAvatarRO(AvatarInfo avatarInfo)
        {
            AvatarInfoDTO dto = new AvatarInfoDTO();
            dto.name = avatarInfo.Name;
            dto.displayName = avatarInfo.DisplayName;
            dto.gender = avatarInfo.AvatarGender.ToString();
            dto.imageData = DataConverter.SpriteToRowData(avatarInfo.Image);
                        
            return dto;
        }

        private void SaveAvatars(IList<AvatarInfo> localAvatars, IList<AvatarInfo> newAvatars)
        {
            IList<AvatarInfo> currentAvatars =
                new List<AvatarInfo>(localAvatars)
                    .Concat(newAvatars)
                    .ToList();

            avatarStore.AvatarInfos = currentAvatars;

            foreach (AvatarInfo newAvatar in currentAvatars)
            {
                string bundlePath = Path.Combine(AvatarAssetPackages.ASSETS_DIRECTORY, newAvatar.Name);

                if (AssetBundleCache.GetBundle(newAvatar.Name) != null)
                {
                    continue;
                }
                
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
                if (assetBundle == null)
                {
                    AppLogger.Warning($"Failed to load AssetBundle {newAvatar.Name}");
                }
                else
                {
                    AssetBundleCache.AddBundle(assetBundle);

                    AvatarFile avatarFile = new AvatarFile();
                    avatarFile.Name = newAvatar.Name;
                    avatarFile.DisplayName = newAvatar.DisplayName;
                    avatarFile.Image = newAvatar.Image;
                    avatarFile.AvatarGender = newAvatar.AvatarGender;
                    avatarFile.Model = AssetBundleUtils.GetMainGameObject(assetBundle);
                    if (avatarFile.Model == null)
                    {
                        AppLogger.Error($"Asset {newAvatar.Name} is corrupted");
                        continue;
                    }

                    avatarStore.AvatarFiles.Add(avatarFile.Name, avatarFile);
                    
                    AppLogger.Log($"Asset {newAvatar.Name} was loaded");
                }
            }
        }

        private async Task SceneDownloading()
        {
            _loadingText.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading.scenes");
            if (sceneStore.SceneInfos.Count == 0)
            {
                IList<SceneInfo> serverScenesInfos = serverAPI.Scene.GetAllSceneInfo();
                IList<SceneInfo> localScenesInfos = SceneAssetPackages.GetMatchingLocalScenes(serverScenesInfos);
                serverScenesInfos = ContainerUtils.RemoveMatchingElements(serverScenesInfos, localScenesInfos);

                IList<SceneInfo> newScenes = await serverAPI.Scene.GetSceneFiles(serverScenesInfos);

                SaveSceneInfos(localScenesInfos, newScenes);
                sceneStore.SceneInfos =
                    localScenesInfos
                        .Concat(newScenes)
                        .ToList();
            }
        }

        private void SaveSceneInfos(IList<SceneInfo> localScenes, IList<SceneInfo> newScenes)
        {
            IList<SceneInfoDTO> currentScenesInfos =
                new List<SceneInfo>(localScenes)
                    .Concat(newScenes)
                    .Select(ToSceneRO)
                    .ToList();

            SceneAssetPackages.SaveSceneInfo(currentScenesInfos);
        }

        private static SceneInfoDTO ToSceneRO(SceneInfo sceneInfo)
        {
            SceneInfoDTO dto = new SceneInfoDTO();
            dto.name = sceneInfo.Name;
            dto.displayName = sceneInfo.DisplayName;
            dto.device = sceneInfo.Device.ToString();
            dto.imageData = DataConverter.SpriteToRowData(sceneInfo.Image);
            dto.sortIndex = sceneInfo.SortIndex;

            return dto;
        }
    }
}