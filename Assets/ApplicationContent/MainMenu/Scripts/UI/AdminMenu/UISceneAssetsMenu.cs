using System;
using System.Collections.Generic;
using System.IO;
using Global.Bundles;
using Global.Files;
using Global.Logger;
using Global.UI;
using Global.UI.LoadingForm;
using Global.UI.ScrollList;
using LDR.SUAI_Metaverse.SDK.Core.Types.Devices;
using Localization;
using MainMenu.Containers;
using MainMenu.UI.ScrollListItems;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;
using UploadingSceneInfo = MainMenu.Containers.UploadingSceneInfo;

namespace MainMenu.UI.AdminMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий меню загрузки ассетов сцен на UI.</para>
    /// </summary>
    public sealed class UISceneAssetsMenu : MonoBehaviour
    {
// В данном меню происходит загрузка файлов из формы Windows
// Данный код не должен попадать в сборку
#if UNITY_EDITOR
        private const string LOCAL_KEY = "MainMenu.text.local";
        private const string SERVER_KEY = "MainMenu.text.server";

        [SerializeField] private UIScrollList _sceneAssetsList;
        [SerializeField] private UISceneAssetItem _assetItemPrefab;
        [SerializeField] private TMP_Text _messageField;
        [SerializeField] private UILoadingForm _loadingForm;

        private APIContainer serverAPI;
        private IList<SceneInfo> sceneInfos = new List<SceneInfo>();
        private ISet<string> bundleFiles = new HashSet<string>();

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            RefreshSceneAssetList();
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }

        private void RefreshSceneAssetList()
        {
            _loadingForm.EnableLoading();
            
            _sceneAssetsList.Clear();
            sceneInfos.Clear();
            
            Dictionary<string, SceneInfo> serverScenes = GetServerScenes();
            
            string directoryName = Path.GetFileName(BundleConstants.ASSET_SCENE_BUNDLES_PATH);
            string[] sceneAssets = FileUtils.GetFilesFromDirectory(BundleConstants.ASSET_SCENE_BUNDLES_PATH, new HashSet<string> { directoryName }, new HashSet<string> { "manifest" });

            AddLocalScenesToList(sceneAssets, ref serverScenes);
            AddServerSceneToList(serverScenes);
            
            _loadingForm.DisableLoading();
        }

        private Dictionary<string, SceneInfo> GetServerScenes()
        {
            IList<SceneInfo> serverSceneList = serverAPI.Scene.GetAllSceneInfo();
            Dictionary<string, SceneInfo> serverScene = new Dictionary<string, SceneInfo>();
            foreach (var info in serverSceneList)
            {
                serverScene.Add(info.Name, info);
            }

            return serverScene;
        }

        private void AddLocalScenesToList(string[] sceneAssets, ref Dictionary<string, SceneInfo> serverScenes)
        {
            bundleFiles.Clear();
            foreach (string sceneAsset in sceneAssets)
            {
                _assetItemPrefab.Name.text = sceneAsset;
                bundleFiles.Add(sceneAsset);
                if (serverScenes.TryGetValue(sceneAsset, out var sceneInfo))
                {
                    _assetItemPrefab.DisplayName.text = sceneInfo.DisplayName;
                    _assetItemPrefab.SortIndex.text = sceneInfo.SortIndex.ToString();
                    SetupDropDown(_assetItemPrefab.Device, sceneInfo.Device);
                    _assetItemPrefab.LoadImage.LoadedImage = sceneInfo.Image;
                    _assetItemPrefab.Status.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", SERVER_KEY);

                    serverScenes.Remove(sceneAsset);
                    sceneInfos.Add(sceneInfo);
                }
                else
                {
                    _assetItemPrefab.DisplayName.text = "";
                    _assetItemPrefab.SortIndex.text = "";
                    SetupDropDown(_assetItemPrefab.Device, Device.PC);
                    _assetItemPrefab.LoadImage.LoadedImage = null;
                    _assetItemPrefab.Status.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", LOCAL_KEY);

                    sceneInfos.Add(
                        new SceneInfo()
                        {
                            Name = sceneAsset
                        }
                    );
                }

                _sceneAssetsList.AddItemWithContent(_assetItemPrefab.gameObject);
            }
        }

        private void AddServerSceneToList(Dictionary<string, SceneInfo> serverScenes)
        {
            foreach (var onlyServerScene in serverScenes.Values)
            {
                _assetItemPrefab.Name.text = onlyServerScene.Name;
                _assetItemPrefab.Name.color = Color.grey;

                _assetItemPrefab.DisplayName.text = onlyServerScene.DisplayName;
                _assetItemPrefab.SortIndex.text = onlyServerScene.SortIndex.ToString();
                SetupDropDown(_assetItemPrefab.Device, onlyServerScene.Device);
                _assetItemPrefab.LoadImage.LoadedImage = onlyServerScene.Image;
                _assetItemPrefab.Status.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", SERVER_KEY);

                _sceneAssetsList.AddItemWithContent(_assetItemPrefab.gameObject);
                sceneInfos.Add(onlyServerScene);
            }
        }

        private static void SetupDropDown(TMP_Dropdown deviceDropDown, Device currentDevice)
        {
            deviceDropDown.ClearOptions();
            foreach (Device device in Enum.GetValues(typeof(Device)))
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = device.ToString();
                deviceDropDown.options.Add(option);
                if (device == currentDevice)
                {
                    int currentIndex = deviceDropDown.options.Count - 1;
                    deviceDropDown.ForceSetValue(currentIndex);
                }
            }
        }

        /// <summary>
        /// <para>Загрузить выбранные ассеты сцен на сервер.</para>
        /// </summary>
        public void UploadSceneAssets()
        {
             _loadingForm.EnableLoading();
            
            IList<GameObject> selected = _sceneAssetsList.GetSelectedItems();
            if (selected.Count == 0)
            {
                return;
            }
            
            IList<UploadingSceneInfo> uploadingScenes = CreateUploadingScenes(selected);

            bool success = serverAPI.Scene.UploadScenes(uploadingScenes);
            
            _loadingForm.DisableLoading();
            
            if (!success)
            {
                AppLogger.Warning("Scenes were not uploaded to the file server");
            }
            else
            {
                RefreshSceneAssetList(); 
            }
        }

        private IList<UploadingSceneInfo> CreateUploadingScenes(IList<GameObject> selected)
        {
            IList<UploadingSceneInfo> uploadingScenes = new List<UploadingSceneInfo>();
            foreach (var item in selected)
            {
                UISceneAssetItem sceneAsset = item.GetComponent<UISceneAssetItem>();

                UploadingSceneInfo sceneInfo = CreateUploadingScene(sceneAsset);

                if (sceneInfo != null)
                {
                    uploadingScenes.Add(sceneInfo);
                }
            }

            return uploadingScenes;
        }

        private UploadingSceneInfo CreateUploadingScene(UISceneAssetItem sceneAsset)
        {
            if (!bundleFiles.Contains(sceneAsset.Name.text))
            {
                return null;
            }

            UploadingSceneInfo sceneInfo = new UploadingSceneInfo();
            sceneInfo.Name = sceneAsset.Name.text;
            sceneInfo.DisplayName = sceneAsset.DisplayName.text;
            sceneInfo.SortIndex = int.Parse(sceneAsset.SortIndex.text);
            string deviceStr = sceneAsset.Device.options[sceneAsset.Device.value].text;
            if (Enum.TryParse(deviceStr, out Device device))
            {
                sceneInfo.Device = device;
            }

            sceneInfo.Image = sceneAsset.LoadImage.LoadedImage;
            sceneInfo.SceneFilePath = $"{BundleConstants.ASSET_SCENE_BUNDLES_PATH}\\{sceneAsset.Name.text}";

            return sceneInfo;
        }

        /// <summary>
        /// <para>Удалить выбранные ассеты сцен с сервера.</para>
        /// </summary>
        public void DeleteSelectedAssetsFromServer()
        {
            _loadingForm.EnableLoading();
            
            IList<GameObject> selected = _sceneAssetsList.GetSelectedItems();
            if (selected.Count == 0)
            {
                return;
            }
            
            List<string> removedAssetNames = new List<string>();
            foreach (var item in selected)
            {
                UISceneAssetItem sceneAsset = item.GetComponent<UISceneAssetItem>();
                removedAssetNames.Add(sceneAsset.Name.text);
            }
            
            bool success = serverAPI.Scene.DeleteManyScenes(removedAssetNames);
            
            _loadingForm.DisableLoading();
            
            if (!success)
            {
                AppLogger.Warning("Failed to delete scenes from the file server");
            }
            else
            {
                RefreshSceneAssetList();
            }
        }
#endif
    }
}