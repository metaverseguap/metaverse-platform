using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using UnityEngine;
using UnityEngine.UI;
using UserSystem.RoleSystem.Core;
using UserSystem.RoleSystem.Types;

namespace MainMenu.UI.AuthMenu
{
    /// <summary>
    /// <para>Скрипт, управляющий меню подключения авторизованного пользователя.</para>
    /// </summary>
    public sealed class UIConnectMenu : MonoBehaviour
    {
        [SerializeField] private Button _adminMenuButton;
        private APIContainer serverAPI;

        private void Start()
        {
            serverAPI = MVNetworkManager.singleton.NetworkStore.FileServer;
        }

        private void OnEnable()
        {
            _adminMenuButton.gameObject.SetActive(false);

            RoleInfo role = GetRole();

            if (role.Permissions.Contains(AppPermission.ADMIN_MENU_ACCESS))
            {
                _adminMenuButton.gameObject.SetActive(true);
            }
        }

        private RoleInfo GetRole()
        {
            RoleInfo role = MVNetworkManager.singleton.NetworkStore.Role;
            if (role == null)
            {
                role = serverAPI.Role.GetMyRole();
                MVNetworkManager.singleton.NetworkStore.Role = role;
            }

            return role;
        }
    }
}