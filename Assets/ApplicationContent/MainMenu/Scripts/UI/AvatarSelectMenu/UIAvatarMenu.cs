using Global.UI;
using Global.UI.LoadingForm;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.UI.AvatarSelectMenu
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
        private AvatarStore avatarStore;

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            avatarStore = EnsureAvatarStore();
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

        private AvatarStore EnsureAvatarStore()
        {
            if (avatarStore == null)
            {
                return MVNetworkManager.singleton.NetworkStore.Avatars;
            }

            return avatarStore;
        }

        private void RefreshAvatarAssetList()
        {
            _avatarDropdown.ClearOptions();

            // Здесь мог бы быть запрос на сервер с получением информации об аватарах
            // Но мы предполагаем, что данный запрос был сделан в сцене загрузки ассетов

            foreach (AvatarInfo avatar in avatarStore.AvatarInfos)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = avatar.DisplayName;
                option.image = avatar.Image;
                _avatarDropdown.options.Add(option);
            }

            _avatarDropdown.Reset();
        }

        /// <summary>
        /// <para>Получить аватар выбранный в данный момент.</para>
        /// </summary>
        /// <returns>аватар выбранный в данный момент</returns>
        public AvatarInfo GetSelectedAvatar()
        {
            return avatarStore.AvatarInfos[_avatarDropdown.value];
        }
    }
}