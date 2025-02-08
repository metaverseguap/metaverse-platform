using System;
using MainMenu.Containers;
using Mirror;
using NetworkCore.ServerInteraction.API;
using Player.Tablet.PC.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkCore.MirrorNetworking.UI
{
    /// <summary>
    /// <para>Кнопка подключения к хосту.</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class UIMirrorConnectToHostButton : MonoBehaviour
    {
        [SerializeField] private UITabletRoomSelectMenu _roomSelectMenu;
        
        private Button button;
        private MVNetworkManager connection;
        private APIContainer serverAPI;
        
        private void OnEnable()
        {
            EnsureButton();
            EnsureNetworkManager();
        }

        private void EnsureButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
                button.onClick.AddListener(StartConnection);
            }
        }
        
        private void EnsureNetworkManager()
        {
            if (MVNetworkManager.IsOnline())
            {
                button.interactable = _roomSelectMenu.GetSelectedHost() != null;
                connection = MVNetworkManager.singleton;
                serverAPI = connection.NetworkStore.FileServer;
            }
            else
            {
                button.interactable = false;
            }
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
        
        private void StartConnection()
        {
            if (connection == null)
            {
                return;
            }

            if (NetworkServer.activeHost)
            {
                serverAPI.Hosts.RemoveHost();
            }

            HostInfo hostInfo = _roomSelectMenu.GetSelectedHost();
            connection.StartClient(new Uri(hostInfo.Uri));
        }
    }
}