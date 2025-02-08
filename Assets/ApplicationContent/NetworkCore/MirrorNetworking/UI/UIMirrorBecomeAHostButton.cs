using MainMenu.Containers;
using Mirror;
using NetworkCore.ServerInteraction.API;
using NetworkCore.Utils;
using Player.Tablet.PC.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkCore.MirrorNetworking.UI
{
    /// <summary>
    /// <para>Кнопка становления хостом.</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class UIMirrorBecomeAHostButton : MonoBehaviour
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
                button.onClick.AddListener(BecomeAHost);
            }
        }

        private void EnsureNetworkManager()
        {
            if (MVNetworkManager.IsOnline())
            {
                button.interactable = true;
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
        
        private void BecomeAHost()
        {
            if (connection == null)
            {
                return;
            }

            SceneInfo connectedScene = _roomSelectMenu.GetSelectedScene();
            if (connectedScene == null)
            {
                return;
            }
            
            if (NetworkServer.activeHost)
            {
                serverAPI.Hosts.RemoveHost();
            }
            else
            {
                connection.StartHost();
            }
            
            connection.NetworkStore.Scenes.CurrentScene = connectedScene;
            connection.ServerChangeScene(connectedScene.Name);
            
            serverAPI.Hosts.BecomeAHost(IPUtils.GetIpAsUrl(), connectedScene.Name);
        }
    }
}