using MainMenu.Containers;
using NetworkCore.MirrorNetworking.Utils;
using Player.Tablet.PC.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkCore.MirrorNetworking.UI.ConnectButtons
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

            SceneInfo connectedScene = _roomSelectMenu.GetSelectedScene();
            if (connectedScene == null)
            {
                return;
            }

            HostInfo hostInfo = _roomSelectMenu.GetSelectedHost();
            if (hostInfo == null)
            {
                return;
            }

            connection.DisconnectFromNetwork();
            connection.BecomeAClient(hostInfo.HostIP, hostInfo.Port, connectedScene);
        }
    }
}