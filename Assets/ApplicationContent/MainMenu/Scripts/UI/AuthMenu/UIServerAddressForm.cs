using Localization;
using NetworkCore.MirrorNetworking;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.ServerInteraction.API;
using NetworkCore.Utils;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.AuthMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий формой указания адреса сервера на UI.</para>
    /// </summary>
    public sealed class UIServerAddressForm : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _serverAddressInputField;
        [SerializeField] private TMP_Text _messageLabel;

        private NetworkDataStore store;
        private APIContainer serverAPI;
        
        private void Start()
        {
            store = MVNetworkManager.singleton.NetworkStore;
            serverAPI = store.FileServer;

            _serverAddressInputField.text = serverAPI.ServerAddress;
        }

        /// <summary>
        /// <para>Изменить адрес файлового сервера, на адрес указанный в input field.</para>
        /// </summary>
        public void ChangeServerAddress()
        {
            string newAddress = _serverAddressInputField.text;
            if (!IPUtils.IsURLValid(newAddress))
            {
                _messageLabel.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.text.invalidUrl");
                _serverAddressInputField.text = serverAPI.ServerAddress;
                return;
            }

            if (_messageLabel.text == LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.text.invalidUrl"))
            {
                _messageLabel.text = "";
            }
            
            serverAPI.ServerAddress = newAddress;
        }
    }
}