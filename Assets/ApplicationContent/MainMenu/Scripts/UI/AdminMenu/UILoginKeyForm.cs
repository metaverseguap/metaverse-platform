using Localization;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UI.Dates;
using UnityEngine;

namespace MainMenu.UI.AdminMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий формой создания ключа авторизации на UI.</para>
    /// </summary>
    public sealed class UILoginKeyForm : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _loginKeyInputField;
        [SerializeField] private DatePicker _dateFromDatePicker;
        [SerializeField] private DatePicker _dateToDatePicker;
        [SerializeField] private TMP_Text _messageField;
        [SerializeField] private UIDeleteLoginKeyFrom _deleteLoginKeyFrom;

        private APIContainer serverAPI;

        private void Start()
        {
            serverAPI = MVNetworkManager.singleton.NetworkStore.FileServer;
        }

        /// <summary>
        /// <para>Отправляет форму на файловый сервер с запросом создания ключа авторизации.</para>
        /// </summary>
        public void SendForm()
        {
            LoginKeyInfo loginKeyInfo = new LoginKeyInfo();
            loginKeyInfo.Key = _loginKeyInputField.text;
            if (_dateFromDatePicker.SelectedDate.HasValue)
            {
                loginKeyInfo.DateFrom = _dateFromDatePicker.SelectedDate.Date;
            }
            else
            {
                _messageField.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.exceptions.emptyDate");
                return;
            }
            if (_dateToDatePicker.SelectedDate.HasValue)
            {
                loginKeyInfo.DateTo = _dateToDatePicker.SelectedDate.Date;
            }
            else
            {
                _messageField.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.exceptions.emptyDate");
                return;
            }
            
            SendRequest(loginKeyInfo);
        }

        private void SendRequest(LoginKeyInfo loginKeyInfo)
        {
            bool success = serverAPI.LoginKey.CreateLoginKey(loginKeyInfo, out string exceptionMessage);
            if (success)
            {
                _deleteLoginKeyFrom.RefreshLoginKeysList();
                _messageField.text = "";
            }
            else
            {
                _messageField.text = exceptionMessage;
            }
        }
    }
}