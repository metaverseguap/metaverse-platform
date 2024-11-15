using System.Collections.Generic;
using Global.UI;
using Global.UI.LoadingForm;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.UI.AvatarMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий меню выбора аватара на UI.</para>
    /// </summary>
    public sealed class UIAvatarMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _avatarDropdown;
        [SerializeField] private Toggle _microphoneToggle;
        [SerializeField] private TMP_Text _messageField;
        [SerializeField] private UILoadingForm _loadingForm;

        private APIContainer serverAPI;
        private IList<AvatarInfo> avatars = new List<AvatarInfo>();

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            RefreshAvatarAssetList();
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                return MVNetworkManager.singleton.FileServer;
            }

            return serverAPI;
        }

        private async void RefreshAvatarAssetList()
        {
            await _loadingForm.EnableLoading();
            
            _avatarDropdown.ClearOptions();
            avatars.Clear();

            avatars = serverAPI.Avatar.GetAllAvatarsInfo();
            foreach (AvatarInfo avatar in avatars)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = avatar.DisplayName;
                option.image = avatar.Image;
                _avatarDropdown.options.Add(option);
            }

            _avatarDropdown.Reset();
            
            _loadingForm.gameObject.SetActive(false);
        }
    }
}