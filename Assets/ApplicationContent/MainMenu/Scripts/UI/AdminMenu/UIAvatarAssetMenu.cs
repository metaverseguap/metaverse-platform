using System;
using System.Collections.Generic;
using System.IO;
using AppAvatars.Types;
using Global.Bundles;
using Global.Files;
using Global.Logger;
using Global.UI;
using Global.UI.LoadingForm;
using Global.UI.ScrollList;
using Localization;
using MainMenu.Containers;
using MainMenu.UI.ScrollList.Items;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.AdminMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий меню загрузки ассетов аватаров на UI.</para>
    /// </summary>
    public sealed class UIAvatarAssetMenu : MonoBehaviour
    {
// В данном меню происходит загрузка файлов из формы Windows
// Данный код не должен попадать в сборку
#if UNITY_EDITOR
        private const string LOCAL_KEY = "MainMenu.text.local";
        private const string SERVER_KEY = "MainMenu.text.server";
        
        [SerializeField] private UIScrollList _avatarAssetsList;
        [SerializeField] private UIAvatarAssetItem _assetItemPrefab;
        [SerializeField] private TMP_Text _messageField;
        [SerializeField] private UILoadingForm _loadingForm;
        
        private APIContainer serverAPI;
        private IList<AvatarInfo> avatarInfos = new List<AvatarInfo>();
        private ISet<string> bundleFiles = new HashSet<string>();

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            RefreshAvatarAssetList();
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }

        private async void RefreshAvatarAssetList()
        {
            await _loadingForm.EnableLoading();
            
            _avatarAssetsList.Clear();
            avatarInfos.Clear();
            
            Dictionary<string, AvatarInfo> serverAvatars = GetServerAvatars();
            
            string directoryName = Path.GetFileName(BundleConstants.ASSET_AVATAR_BUNDLES_PATH);
            string[] avatarAssets = FileUtils.GetFilesFromDirectory(BundleConstants.ASSET_AVATAR_BUNDLES_PATH, new HashSet<string> { directoryName }, new HashSet<string> { "manifest" });

            AddLocalAvatarsToList(avatarAssets, ref serverAvatars);
            AddServerAvatarToList(serverAvatars);
            
            _loadingForm.gameObject.SetActive(false);
        }

        private Dictionary<string, AvatarInfo> GetServerAvatars()
        {
            IList<AvatarInfo> serverAvatarList = serverAPI.Avatar.GetAllAvatarsInfo();
            Dictionary<string, AvatarInfo> serverAvatars = new Dictionary<string, AvatarInfo>();
            foreach (var info in serverAvatarList)
            {
                serverAvatars.Add(info.Name, info);
            }

            return serverAvatars;
        }

        private void AddLocalAvatarsToList(string[] avatarAssets, ref Dictionary<string, AvatarInfo> serverAvatars)
        {
            bundleFiles.Clear();
            foreach (string avatarAsset in avatarAssets)
            {
                _assetItemPrefab.Name.text = avatarAsset;
                _assetItemPrefab.Name.color = Color.white;
                bundleFiles.Add(avatarAsset);
                if (serverAvatars.TryGetValue(avatarAsset, out var avatarInfo))
                {
                    _assetItemPrefab.DisplayName.text = avatarInfo.DisplayName;
                    SetupDropDown(_assetItemPrefab.Gender, avatarInfo.AvatarGender);
                    _assetItemPrefab.LoadImage.LoadedImage = avatarInfo.Image;
                    _assetItemPrefab.Status.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", SERVER_KEY);

                    serverAvatars.Remove(avatarAsset);
                    avatarInfos.Add(avatarInfo);
                }
                else
                {
                    _assetItemPrefab.DisplayName.text = "";
                    SetupDropDown(_assetItemPrefab.Gender, Gender.MALE);
                    _assetItemPrefab.LoadImage.LoadedImage = null;
                    _assetItemPrefab.Status.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", LOCAL_KEY);

                    avatarInfos.Add(
                        new AvatarInfo()
                        {
                            Name = avatarAsset
                        }
                    );
                }

                _avatarAssetsList.AddItemWithContent(_assetItemPrefab.gameObject);
            }
        }

        private void AddServerAvatarToList(Dictionary<string, AvatarInfo> serverAvatars)
        {
            foreach (var onlyServerAvatar in serverAvatars.Values)
            {
                _assetItemPrefab.Name.text = onlyServerAvatar.Name;
                _assetItemPrefab.Name.color = Color.grey;

                _assetItemPrefab.DisplayName.text = onlyServerAvatar.DisplayName;
                SetupDropDown(_assetItemPrefab.Gender, onlyServerAvatar.AvatarGender);
                _assetItemPrefab.LoadImage.LoadedImage = onlyServerAvatar.Image;
                _assetItemPrefab.Status.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", SERVER_KEY);

                _avatarAssetsList.AddItemWithContent(_assetItemPrefab.gameObject);
                avatarInfos.Add(onlyServerAvatar);
            }
        }
        
        private static void SetupDropDown(TMP_Dropdown genderDropDown, Gender currentGender)
        {
            genderDropDown.ClearOptions();
            foreach (Gender gender in Enum.GetValues(typeof(Gender)))
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = gender.ToString();
                genderDropDown.options.Add(option);
                if (gender == currentGender)
                {
                    int currentIndex = genderDropDown.options.Count - 1;
                    genderDropDown.ForceSetValue(currentIndex);
                }
            }
        }
        
        /// <summary>
        /// <para>Загрузить выбранные ассеты аватаров на сервер.</para>
        /// </summary>
        public async void UploadAvatarAssets()
        {
            await _loadingForm.EnableLoading();
            
            IList<GameObject> selected = _avatarAssetsList.GetSelectedItems();
            if (selected.Count == 0)
            {
                return;
            }
            
            IList<UploadAvatarInfo> uploadingAvatars = CreateUploadingAvatars(selected);

            bool success = serverAPI.Avatar.UploadAvatars(uploadingAvatars);
            
            _loadingForm.gameObject.SetActive(false);
            
            if (!success)
            {
                AppLogger.Warning("Avatars were not uploaded to the file server");
            }
            else
            {
                RefreshAvatarAssetList(); 
            }
        }

        private IList<UploadAvatarInfo> CreateUploadingAvatars(IList<GameObject> selected)
        {
            _messageField.text = "";
            
            IList<UploadAvatarInfo> uploadingAvatars = new List<UploadAvatarInfo>();
            foreach (var item in selected)
            {
                UIAvatarAssetItem avatarAsset = item.GetComponent<UIAvatarAssetItem>();

                UploadAvatarInfo avatarInfo = CreateUploadingAvatar(avatarAsset);

                if (avatarInfo != null)
                {
                    uploadingAvatars.Add(avatarInfo);
                }
            }

            return uploadingAvatars;
        }

        private UploadAvatarInfo CreateUploadingAvatar(UIAvatarAssetItem avatarAsset)
        {
            if (!bundleFiles.Contains(avatarAsset.Name.text))
            {
                return null;
            }

            if (avatarAsset.Status.text == LocalizationUtils.GetStringFromTable("MenuLocaleTable", SERVER_KEY))
            {
                _messageField.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.message.canNotLoadServerAssets");
                return null;
            }

            UploadAvatarInfo avatarInfo = new UploadAvatarInfo();
            avatarInfo.Name = avatarAsset.Name.text;
            avatarInfo.DisplayName = avatarAsset.DisplayName.text;
            string genderStr = avatarAsset.Gender.options[avatarAsset.Gender.value].text;
            if (Enum.TryParse(genderStr, out Gender gender))
            {
                avatarInfo.AvatarGender = gender;
            }

            avatarInfo.Image = avatarAsset.LoadImage.LoadedImage;
            avatarInfo.AvatarFilePath = $"{BundleConstants.ASSET_AVATAR_BUNDLES_PATH}\\{avatarAsset.Name.text}";

            return avatarInfo;
        }
        
        /// <summary>
        /// <para>Удалить выбранные ассеты аватаров с сервера.</para>
        /// </summary>
        public async void DeleteSelectedAssetsFromServer()
        {
            await _loadingForm.EnableLoading();
            
            IList<GameObject> selected = _avatarAssetsList.GetSelectedItems();
            if (selected.Count == 0)
            {
                return;
            }
            
            List<string> removedAssetNames = new List<string>();
            foreach (var item in selected)
            {
                UIAvatarAssetItem avatarAsset = item.GetComponent<UIAvatarAssetItem>();
                removedAssetNames.Add(avatarAsset.Name.text);
            }
            
            bool success = serverAPI.Avatar.DeleteManyAvatars(removedAssetNames);
            
            _loadingForm.gameObject.SetActive(false);
            
            if (!success)
            {
                AppLogger.Warning("Failed to delete avatars from the file server");
            }
            else
            {
                RefreshAvatarAssetList();
            }
        }
#endif
    }
}