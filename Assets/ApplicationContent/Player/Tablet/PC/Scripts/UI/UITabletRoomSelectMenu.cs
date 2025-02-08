using System.Collections.Generic;
using Global.Logger;
using Global.UI;
using Global.UI.ScrollList;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API;
using Player.Tablet.UI.ScrollListItems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserSystem.Types;

namespace Player.Tablet.PC.UI
{
    /// <summary>
    /// <para>Скрипт, управляющий меню выбора комнаты на UI.</para>
    /// </summary>
    public sealed class UITabletRoomSelectMenu : MonoBehaviour
    {
        private const string SCRIPT_FUNCTION = "Host list processing";

        [SerializeField] private TMP_Dropdown _sceneDropdown;

        [Header("Scroll List")]
        [SerializeField] private UIScrollList _hostsList;
        [SerializeField] private UIHostItem _hostItemPrefab;

        [Header("Buttons")] 
        [SerializeField] private Button _becomeAHostButton;
        [SerializeField] private Button _connectToHostButton;

        private APIContainer serverAPI;

        private IList<SceneInfo> sceneInfo = new List<SceneInfo>();
        private IList<HostInfo> hosts = new List<HostInfo>();
        private UserInfo currentUser;

        private void OnEnable()
        {
            serverAPI = EnsureServerAPI();
            currentUser = EnsureUserInfo(serverAPI);
            RefreshMenu();
        }

        private void OnDestroy()
        {
            _hostsList.OnItemChangeValue -= ActivateConnectButton;
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null)
            {
                if (MVNetworkManager.IsOffline())
                {
                    AppLogger.Error(string.Format("Attempting to use online functions ({0}) in offline mode", SCRIPT_FUNCTION));
                    return null;
                }

                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }

        private UserInfo EnsureUserInfo(APIContainer serverAPI)
        {
            if (currentUser != null)
            {
                return currentUser;
            }

            if (serverAPI == null)
            {
                return null;
            }

            return serverAPI.User.GetMyUser();
        }

        private void RefreshMenu()
        {
            RefreshSceneInfo();
            RefreshHostsList();
            RefreshButtons();
        }

        private void RefreshSceneInfo()
        {
            _sceneDropdown.ClearOptions();
            sceneInfo = serverAPI.Scene.GetAllSceneInfo();

            foreach (SceneInfo sceneInfo in sceneInfo)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = sceneInfo.DisplayName.ToString();
                option.image = sceneInfo.Image;
                _sceneDropdown.options.Add(option);
            }

            _sceneDropdown.Reset();
        }

        private void RefreshHostsList()
        {
            if (sceneInfo.Count == 0 || sceneInfo.Count != _sceneDropdown.options.Count)
            {
                return;
            }

            _hostsList.Clear();
            hosts.Clear();

            hosts = serverAPI.Hosts.GetHostsBySceneName(sceneInfo[_sceneDropdown.value].Name);
            foreach (var host in hosts)
            {
                if (host.Login == currentUser.Login)
                {
                    continue;
                }
                
                _hostItemPrefab.Hostname.text = host.DisplayName;

                _hostsList.AddItemWithContent(_hostItemPrefab.gameObject);
            }
        }

        private void RefreshButtons()
        {
            _becomeAHostButton.interactable =
                sceneInfo.Count > 0
                && sceneInfo.Count == _sceneDropdown.options.Count
                && MVNetworkManager.IsOnline();

            _hostsList.OnItemChangeValue += ActivateConnectButton;
        }

        private void ActivateConnectButton()
        {
            _connectToHostButton.interactable =
                _hostsList.GetSelectedItemsIndexes().Count > 0
                && MVNetworkManager.IsOnline();
        }

        /// <summary>
        /// <para>Получить информацию о хосте выбранным в данный момент.</para>
        /// </summary>
        /// <returns>информация о хосте выбранном в данный момент</returns>
        public HostInfo GetSelectedHost()
        {
            IList<int> selectedIndexed = _hostsList.GetSelectedItemsIndexes();
            if (selectedIndexed.Count == 0)
            {
                return null;
            }

            return hosts[selectedIndexed[0]];
        }

        /// <summary>
        /// <para>Получить информацию о сцене выбранной в данный момент.</para>
        /// </summary>
        /// <returns>информация о сцене выбранной в данный момент</returns>
        public SceneInfo GetSelectedScene()
        {
            if (sceneInfo.Count == 0 || sceneInfo.Count != _sceneDropdown.options.Count)
            {
                return null;
            }

            return sceneInfo[_sceneDropdown.value];
        }
    }
}