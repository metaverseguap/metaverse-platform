using Global.UI.AreYouSureWindow;
using Localization;
using NetworkCore.MirrorNetworking.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NetworkCore.MirrorNetworking.UI.ConnectButtons
{
    /// <summary>
    /// <para>Кнопка возвращения в офлайн сцену.</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class UIBackToOfflineScene : MonoBehaviour
    {
        [SerializeField] private UIAreYouSureWindow _areYouSureWindow;
        
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
                button.onClick.AddListener(ShowWarning);
            }
        }

        private void EnsureNetworkManager()
        {
            if (MVNetworkManager.IsOnline())
            {
                connection = MVNetworkManager.singleton;
                
                string offlineScene = connection.onlineScene;
                string currentScene = SceneManager.GetActiveScene().path;
                button.interactable = currentScene != offlineScene;
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

        private void ShowWarning()
        {
            string message = LocalizationUtils.GetStringFromTable("TabletLocaleTable", "Tablet.label.toOfflineScene");
            _areYouSureWindow.ShowWindow(message, ToOfflineScene);
        }

        private void ToOfflineScene()
        {
            if (connection == null)
            {
                return;
            }

            connection.DisconnectFromNetwork();
            connection.StartOfflineScene();
        }
    }
}