using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.AuthMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий формой регистрации на ui.</para>
    /// </summary>
    public sealed class UIRegistrationForm : MonoBehaviour
    {
        [Header("Form fields")]
        [SerializeField] private TMP_InputField _loginField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private TMP_InputField _passwordRepeatField;
        [SerializeField] private TMP_InputField _nicknameField;
        [SerializeField] private TMP_InputField _registrationCodeField;
        [SerializeField] private TMP_Text _messageLabel;
        [Header("After success")]
        [SerializeField] private PanelSwitcher _panelSwitcher;
        
        private APIContainer serverAPI;

        private void Start()
        {
            serverAPI = MVNetworkManager.singleton.NetworkStore.FileServer;
        }
        
        /// <summary>
        /// <para>Отправляет форму на файловый сервер с запросом регистрации.</para>
        /// </summary>
        public void SendForm()
        {
            RegistrationInfo registrationInfo = new RegistrationInfo();
            registrationInfo.Login = _loginField.text;
            registrationInfo.Password = _passwordField.text;
            registrationInfo.RepeatPassword = _passwordRepeatField.text;
            registrationInfo.NickName = _nicknameField.text;
            registrationInfo.RegistrationKey = _registrationCodeField.text;
            
            bool success = serverAPI.Auth.Registration(registrationInfo, out string message);
            
            if (success)
            {
                ClearForm();
                _panelSwitcher.SwitchPanels();
            }
            else
            {
                _messageLabel.text = message;
            }
        }

        private void ClearForm()
        {
            _messageLabel.text = "";
            _loginField.text = "";
            _passwordField.text = "";
            _passwordRepeatField.text = "";
            _nicknameField.text = "";
            _registrationCodeField.text = "";
        }
    }  
}
