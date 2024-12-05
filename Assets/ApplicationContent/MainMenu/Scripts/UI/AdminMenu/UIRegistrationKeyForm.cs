using System;
using System.Collections.Generic;
using Localization;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UI.Dates;
using UnityEngine;
using UserSystem.RoleSystem.Core;
using UserSystem.RoleSystem.Types;

namespace MainMenu.UI.AdminMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий формой создания ключа регистрации на UI.</para>
    /// </summary>
    public sealed class UIRegistrationKeyForm : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _registrationKeyInputField;
        [SerializeField] private DatePicker _dateFromDatePicker;
        [SerializeField] private DatePicker _dateToDatePicker;
        [SerializeField] private TMP_InputField _organisationInputField;
        [SerializeField] private TMP_Dropdown _serverRoleDropdown;
        [SerializeField] private TMP_Dropdown _applicationRoleDropdown;
        [SerializeField] private TMP_Text _messageField;
        [SerializeField] private UIDeleteRegistrationKeyForm _deleteRegistrationKeyForm;
        
        private APIContainer serverAPI;

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            RefreshRoles();
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }

        private void RefreshRoles()
        {
            RefreshAppRoles();
            RefreshServerRoles();
        }

        private void RefreshAppRoles()
        {
            _applicationRoleDropdown.ClearOptions();
            IList<RoleInfo> roleInfos = serverAPI.Role.GetRoles();

            foreach (RoleInfo roleInfo in roleInfos)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = roleInfo.Name.ToString();
                _applicationRoleDropdown.options.Add(option);
            }
        }

        private void RefreshServerRoles()
        {
            _serverRoleDropdown.ClearOptions();
            
            IList<ServerRoleInfo> serverRoleInfos = serverAPI.Role.GetServerRoles();

            foreach (ServerRoleInfo serverRoleInfo in serverRoleInfos)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = serverRoleInfo.Name;
                _serverRoleDropdown.options.Add(option);
            }
        }
        
        /// <summary>
        /// <para>Отправляет форму на файловый сервер с запросом создания ключа регистрации.</para>
        /// </summary>
        public void SendForm()
        {
            RegistrationKeyInfo regKeyInfo = new RegistrationKeyInfo();
            regKeyInfo.Key = _registrationKeyInputField.text;
            if (_dateFromDatePicker.SelectedDate.HasValue)
            {
                regKeyInfo.DateFrom = _dateFromDatePicker.SelectedDate.Date;
            }
            else
            {
                _messageField.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.exceptions.emptyDate");
                return;
            }
            if (_dateToDatePicker.SelectedDate.HasValue)
            {
                regKeyInfo.DateTo = _dateToDatePicker.SelectedDate.Date;
            }
            else
            {
                _messageField.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.exceptions.emptyDate");
                return;
            }
            
            regKeyInfo.Organization = _organisationInputField.text;
            regKeyInfo.ServerRole = _serverRoleDropdown.options[_serverRoleDropdown.value].text;
            
            string roleStr = _applicationRoleDropdown.options[_applicationRoleDropdown.value].text;
            if (Enum.TryParse(roleStr, out AppRole role))
            {
                regKeyInfo.Role = role;
            }

            SendRequest(regKeyInfo);
        }

        private void SendRequest(RegistrationKeyInfo regKeyInfo)
        {
            bool success = serverAPI.RegistrationKey.CreateRegistrationKey(regKeyInfo, out string exceptionMessage);
            if (success)
            {
                _deleteRegistrationKeyForm.RefreshRegistrationKeysList();
                _messageField.text = "";
            }
            else
            {
                _messageField.text = exceptionMessage;
            }
        }
    }
}