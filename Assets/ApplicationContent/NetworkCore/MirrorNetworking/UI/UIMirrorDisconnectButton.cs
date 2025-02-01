using Global.UI.AreYouSureWindow;
using Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NetworkCore.MirrorNetworking.UI
{
    /// <summary>
    /// <para>Кнопка отключения от mirror и возврата в главное меню.</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class UIMirrorDisconnectButton : MonoBehaviour
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

        private void ShowWarning()
        {
            string message = LocalizationUtils.GetStringFromTable("TabletLocaleTable", "Tablet.label.exitMetaverse");
            _areYouSureWindow.ShowWindow(message, StartDisconnection);
        }

        private void StartDisconnection()
        {
            if (connection == null)
            {
                return;
            }
                
            connection.StopHost();
            string menuScene = connection.NetworkStore.Scenes.MenuSceneName;
            SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
        }
    }
}