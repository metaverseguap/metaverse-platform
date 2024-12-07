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

        private void OnEnable()
        {
            EnsureServerAPI();
            
            _adminMenuButton.gameObject.SetActive(false);

            RoleInfo role = serverAPI.Role.GetMyRole();

            if (role.Permissions.Contains(AppPermission.ADMIN_MENU_ACCESS))
            {
                _adminMenuButton.gameObject.SetActive(true);
            }
        }

        private void EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                serverAPI = MVNetworkManager.singleton.NetworkStore.FileServer;
            }
        }
    }
}