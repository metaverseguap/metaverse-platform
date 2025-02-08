using System.Collections.Generic;
using Global.UI;
using Global.UI.ScrollList;
using MainMenu.UI.ScrollListItems;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using TMPro;
using UnityEngine;
using UserSystem.RoleSystem.Core;
using UserSystem.RoleSystem.Types;

namespace MainMenu.UI.AdminMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий меню изменения ролей на UI.</para>
    /// </summary>
    public sealed class UIRoleMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _roleDropdown;
        [SerializeField] private UIScrollList _permissionsList;
        [SerializeField] private UIPermissionItem _permissionItemPrefab;
        [SerializeField] private TMP_Text _messageField;
        
        private APIContainer serverAPI;
        private IList<RoleInfo> rolesInfo = new List<RoleInfo>();
        private IList<AppPermission> permissions = new List<AppPermission>();
        
        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            RefreshMenu();
        }
        
        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }
        
        private void RefreshMenu()
        {
            RefreshRoles();
            RefreshPermissions();
            SelectActivePermissions();
        }
        
        private void RefreshRoles()
        {
            _roleDropdown.ClearOptions();
            rolesInfo = serverAPI.Role.GetRoles();

            foreach (RoleInfo roleInfo in rolesInfo)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = roleInfo.Name.ToString();
                _roleDropdown.options.Add(option);
            }

            _roleDropdown.Reset();
        }
        
        private void RefreshPermissions()
        {
            _permissionsList.Clear();
            permissions = serverAPI.Role.GetPermissions();

            foreach (AppPermission permission in permissions)
            {
                _permissionItemPrefab.Name.text = permission.ToString();
                _permissionsList.AddItemWithContent(_permissionItemPrefab.gameObject);
            }
        }
        
        /// <summary>
        /// <para>Делает активными разрешения выбранной роли.</para>
        /// </summary>
        public void SelectActivePermissions()
        {
            int selectedRoleIndex = _roleDropdown.value;
            RoleInfo selectedRole = rolesInfo[selectedRoleIndex];
            List<AppPermission> activePermissions = selectedRole.Permissions;
            
            _permissionsList.DeselectAll();
            if (activePermissions != null)
            {
                foreach (var permission in activePermissions)
                {
                    int activeIndex = permissions.IndexOf(permission);
                    _permissionsList.SelectItem(activeIndex);
                }
            }
        }

        /// <summary>
        /// <para>Обновляет <see cref="AppRole">роли</see> и <see cref="AppPermission">права (разрешения)</see> на сервере согласно состоянию файлов в приложении.</para>
        ///
        /// <remarks>Роли хранятся в файле <c>AppRole.cs</c>, а права (разрешения) в файле <c>AppPermission.cs</c>.
        /// После добавления, удаления ролей из этих файлов необходимо обновить состояние ролей и разрешений на сервере</remarks>
        /// </summary>
        public void UpdateServerState()
        {
            bool success = serverAPI.Role.Refresh();
            if (success)
            {
                RefreshMenu();
            }
        }

        /// <summary>
        /// <para>Обновляет <see cref="AppPermission">права (разрешения)</see> для выбранной в данный момент <see cref="AppRole">роли</see>.</para>
        /// </summary>
        public void UpdateRolePermissions()
        {
            if (rolesInfo == null || rolesInfo.Count == 0)
            {
                return;
            }
            
            int roleIndex = _roleDropdown.value;
            RoleInfo role = rolesInfo[roleIndex];
            role.Permissions = new List<AppPermission>();
            IList<int> permissionIndexes = _permissionsList.GetSelectedItemsIndexes();

            foreach (int index in permissionIndexes)
            {
                role.Permissions.Add(permissions[index]);
            }

            bool success = serverAPI.Role.UpsertRole(role, out string exceptionMessage);
            if (success)
            {
                _messageField.text = "";
                SelectActivePermissions();
            }
            else
            {
                _messageField.text = exceptionMessage;
            }
        }
    }
}