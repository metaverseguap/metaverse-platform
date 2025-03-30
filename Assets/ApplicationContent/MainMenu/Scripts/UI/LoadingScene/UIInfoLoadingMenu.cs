using System;
using System.Collections;
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
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.ServerInteraction.API;
using NetworkCore.ServerInteraction.Type.Avatar;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu.UI.LoadingScene
{
    /// <summary>
    /// <para>Скрипт загружающий информацию о аватарах и сценах с файлового сервера.</para>
    /// Скрипт так же загружает файлы аватаров.
    /// </summary>
    public sealed class UIInfoLoadingMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] [Scene] private string _selectAvatarScene;

        private APIContainer serverAPI;
        private AvatarStore avatarStore;
        private SceneStore sceneStore;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _loadingText.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading");
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            MVNetworkManager networkManager = MVNetworkManager.singleton;
            avatarStore = networkManager.NetworkStore.Avatars;
            sceneStore = networkManager.NetworkStore.Scenes;
            serverAPI = networkManager.NetworkStore.FileServer;

            // Загрузка ассетов с сервера в асинхронном режиме
            StartCoroutine(DownloadAssets());
        }

        private IEnumerator DownloadAssets()
        {
            // Ждем один кадр, что бы сцена загрузилась
            yield return null;
            
            string loadingText = _loadingText.text;

            loadingText = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading.avatars");
            yield return ExecuteThenAwaitFrame(() => _loadingText.text = loadingText);
            
            var avatarDownloading = AvatarDownloading();
            yield return new WaitUntil(() => avatarDownloading.IsCompleted);

            loadingText = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading.scenes");
            yield return ExecuteThenAwaitFrame(() => _loadingText.text = loadingText);

            SceneInfoDownloading();

            // Ждем один кадр
            yield return null;

            SceneManager.LoadScene(_selectAvatarScene, LoadSceneMode.Single);
        }

        private IEnumerator ExecuteThenAwaitFrame(Action callback = null)
        {
            callback?.Invoke();

            // Ждем один кадр, что бы сцена загрузилась
            yield return null;
        }

        private async Task AvatarDownloading()
        {
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
                if (AssetBundleCache.GetBundle(newAvatar.Name) != null)
                {
                    continue;
                }
                
                string bundlePath = Path.Combine(AvatarAssetPackages.ASSETS_DIRECTORY, newAvatar.Name);
                
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

        private void SceneInfoDownloading()
        {
            if (sceneStore.SceneInfos.Count == 0)
            {
                IList<SceneInfo> serverScenesInfos = serverAPI.Scene.GetAllSceneInfo();
                IList<SceneInfo> localScenesInfos = SceneAssetPackages.GetMatchingLocalScenes(serverScenesInfos);
                serverScenesInfos = ContainerUtils.RemoveMatchingElements(serverScenesInfos, localScenesInfos);

                sceneStore.SceneInfos =
                    localScenesInfos
                        .Concat(serverScenesInfos)
                        .ToList();
            }
        }
    }
}