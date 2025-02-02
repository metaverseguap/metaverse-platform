using MainMenu.Containers;
using MainMenu.UI.AvatarSelectMenu;
using OfflineScene;
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
        [SerializeField] private UIAvatarMenu _avatarMenu;
        
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
                button.interactable = true;
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

            AvatarInfo avatarInfo = _avatarMenu.GetSelectedAvatar();
            connection.NetworkStore.Player.AvatarName = avatarInfo.Name;

            connection.StartHost();
            connection.NetworkStore.Scenes.CurrentScene = OfflineSceneConstants.SCENE_INFO;
            connection.ServerChangeScene(connection.offlineScene);
        }
    }
}