using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Global.Logger;
using Global.UI;
using Global.UI.ScrollList;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.MirrorNetworking.Containers.Store;
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
        private NetworkDataStore store;

        private IList<SceneInfo> sceneInfo = new List<SceneInfo>();
        private IList<HostInfo> hosts = new List<HostInfo>();
        private UserInfo currentUser;

        private Coroutine refreshCoroutine;
        private bool enableMenuRefresh = false;

        private void OnEnable()
        {
            EnsureNetwork();
            currentUser = EnsureUserInfo(serverAPI);
            enableMenuRefresh = true;
            if (refreshCoroutine == null)
            {
                refreshCoroutine = StartCoroutine(RefreshMenuLoop());
            }

            _hostsList.OnItemChangeValue += ActivateConnectButton;
        }

        private void OnDisable()
        {
            enableMenuRefresh = false;
            if (refreshCoroutine != null)
            {
                StopCoroutine(refreshCoroutine);
                refreshCoroutine = null;
            }

            _hostsList.OnItemChangeValue -= ActivateConnectButton;
        }

        private void EnsureNetwork()
        {
            if (serverAPI == null)
            {
                if (MVNetworkManager.IsOffline())
                {
                    AppLogger.Error(string.Format("Attempting to use online functions ({0}) in offline mode", SCRIPT_FUNCTION));
                    return;
                }

                MVNetworkManager connection = MVNetworkManager.singleton;
                store = connection.NetworkStore;
                serverAPI = store.FileServer;
            }
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

        private IEnumerator RefreshMenuLoop()
        {
            while (store == null || serverAPI == null)
            {
                EnsureNetwork();
                yield return null;
            }

            ClearMenu();

            int updateInterval = serverAPI.Hosts.HostRefreshInterval;

            while (enableMenuRefresh)
            {
                yield return RefreshSceneInfo();
                yield return RefreshHostsList();
                RefreshButtons();

                yield return new WaitForSeconds(updateInterval);
            }
        }

        private void ClearMenu()
        {
            _sceneDropdown.ClearOptions();
            sceneInfo.Clear();
            _hostsList.Clear();
            hosts.Clear();
            RefreshButtons();
        }

        private IEnumerator RefreshSceneInfo()
        {
            Task<IList<SceneInfo>> request = GetSceneInfo();
            yield return new WaitUntil(() => request.IsCompleted);
            IList<SceneInfo> newScenes = request.Result;

            if (!NeedRefresh(newScenes))
            {
                yield break;
            }

            _sceneDropdown.ClearOptions();
            sceneInfo.Clear();
            sceneInfo = newScenes;

            foreach (SceneInfo info in sceneInfo)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = info.DisplayName.ToString();
                option.image = info.Image;
                _sceneDropdown.options.Add(option);
            }

            _sceneDropdown.Reset();
        }

        private bool NeedRefresh(IList<SceneInfo> newScenes)
        {
            if (newScenes.Count != sceneInfo.Count)
            {
                return true;
            }

            for (int i = 0; i < newScenes.Count; i++)
            {
                if (newScenes[i].Name != sceneInfo[i].Name)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<IList<SceneInfo>> GetSceneInfo()
        {
            IList<SceneInfo> result = new List<SceneInfo>();

            Dictionary<string, SceneInfo> cachedScenes = new Dictionary<string, SceneInfo>();
            foreach (SceneInfo scene in store.FileStore.Scenes.SceneInfos)
            {
                cachedScenes.Add(scene.Name, scene);
            }

            var needRefresh = await NeedRefresh(cachedScenes);

            if (!needRefresh)
            {
                return store.FileStore.Scenes.SceneInfos;
            }

            IList<SceneInfo> request = await serverAPI.Scene.GetAllSceneInfoAsync();
            foreach (SceneInfo scene in request)
            {
                if (cachedScenes.ContainsKey(scene.Name))
                {
                    scene.CachedPath = cachedScenes[scene.Name].CachedPath;
                }

                result.Add(scene);
            }

            store.FileStore.Scenes.SceneInfos = result;

            return result;
        }

        private async Task<bool> NeedRefresh(Dictionary<string, SceneInfo> cachedScenes)
        {
            IList<SceneUpdateInfo> updateInfos = await serverAPI.Scene.GetAllSceneUpdatesInfoAsync();

            if (cachedScenes.Count != updateInfos.Count)
            {
                return true;
            }

            foreach (SceneUpdateInfo updateInfo in updateInfos)
            {
                if (!cachedScenes.ContainsKey(updateInfo.Name))
                {
                    return true;
                }

                if (cachedScenes[updateInfo.Name].UpdateDate != updateInfo.UpdateDate)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator RefreshHostsList()
        {
            if (sceneInfo.Count == 0 || sceneInfo.Count != _sceneDropdown.options.Count)
            {
                yield break;
            }

            Task<IList<HostInfo>> request = serverAPI.Hosts.GetHostsBySceneNameAsync(sceneInfo[_sceneDropdown.value].Name);
            yield return new WaitUntil(() => request.IsCompleted);
            IList<HostInfo> newHosts = request.Result;

            if (!NeedRefresh(newHosts))
            {
                yield break;
            }

            _hostsList.Clear();
            hosts.Clear();
            foreach (var host in newHosts)
            {
                if (host.Login == currentUser.Login)
                {
                    continue;
                }

                hosts.Add(host);

                _hostItemPrefab.Hostname.text = host.DisplayName;
                _hostsList.AddItemWithContent(_hostItemPrefab.gameObject);
            }
        }

        private bool NeedRefresh(IList<HostInfo> newHosts)
        {
            if (newHosts.Count != hosts.Count)
            {
                return true;
            }

            for (int i = 0; i < newHosts.Count; i++)
            {
                if (newHosts[i].Login != hosts[i].Login)
                {
                    return true;
                }
            }

            return false;
        }

        private void RefreshButtons()
        {
            _becomeAHostButton.interactable =
                sceneInfo.Count > 0
                && sceneInfo.Count == _sceneDropdown.options.Count
                && MVNetworkManager.IsOnline();
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