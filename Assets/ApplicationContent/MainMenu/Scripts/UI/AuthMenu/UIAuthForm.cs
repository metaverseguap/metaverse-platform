using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.AuthMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий формой авторизации на UI.</para>
    /// </summary>
    public sealed class UIAuthForm : MonoBehaviour
    {
        [Header("Form fields")]
        [SerializeField] private TMP_InputField _loginField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private TMP_InputField _authCodeField;
        [SerializeField] private TMP_Text _messageLabel;
        [Header("After success")]
        [SerializeField] private PanelSwitcher _panelSwitcher;

        private APIContainer serverAPI;

        private void Start()
        {
            serverAPI = MVNetworkManager.singleton.NetworkStore.FileServer;
        }

        private void OnEnable()
        {
            if (MVNetworkManager.singleton != null)
            {
                MVNetworkManager.singleton.NetworkStore.MyPlayerInfo.Clear(); 
            }
        }

        /// <summary>
        /// <para>Отправляет форму на файловый сервер с запросом авторизации.</para>
        /// </summary>
        public void SendForm()
        {
            bool success = serverAPI.Auth.Login(_loginField.text, _passwordField.text, _authCodeField.text, out string message);
            
            if (success)
            {
                _messageLabel.text = "";
                _loginField.text = "";
                _passwordField.text = "";
                _authCodeField.text = "";
                _panelSwitcher.SwitchPanels();
            }
            else
            {
                _messageLabel.text = message;
            }
        }
    }
}