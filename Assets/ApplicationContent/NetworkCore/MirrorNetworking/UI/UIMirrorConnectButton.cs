using UnityEngine;
using UnityEngine.UI;

namespace NetworkCore.MirrorNetworking.UI
{
    /// <summary>
    /// <para>Кнопка подключения к Mirror.</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class UIMirrorConnectButton : MonoBehaviour
    {
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
            if (MVNetworkManager.singleton == null)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
                connection = MVNetworkManager.singleton;
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

            connection.StartHost();
            connection.ServerChangeScene(connection.offlineScene);
        }
    }
}