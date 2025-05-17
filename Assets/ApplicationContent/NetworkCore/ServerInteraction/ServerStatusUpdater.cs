using System.Collections;
using System.Threading.Tasks;
using NetworkCore.MirrorNetworking;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.ServerInteraction.API;
using UnityEngine;

namespace NetworkCore.ServerInteraction
{
    /// <summary>
    /// <para>Компонент, обновляющий статус игрока на файловом сервере.</para>
    /// </summary>
    public sealed class ServerStatusUpdater : MonoBehaviour
    {
        #region Singleton

        private static ServerStatusUpdater instance = null;

        private void Awake()
        {
            if (!InitSingleton()) return;
        }

        private bool InitSingleton()
        {
            if (instance != null && instance == this)
                return true;

            if (instance != null)
            {
                Destroy(gameObject);

                return false;
            }

            instance = this;
            if (Application.isPlaying)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            return true;
        }

        #endregion

        private NetworkDataStore networkStore;
        private APIContainer serverAPI;
        
        private Coroutine updateCoroutine;
        
        private bool enableStatusUpdate = false;

        private void OnEnable()
        {
            enableStatusUpdate = true;
            if (updateCoroutine == null)
            {
                updateCoroutine = StartCoroutine(SendStatusUpdateLoop());
            }
        }

        private void OnDisable()
        {
            enableStatusUpdate = false;
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
        }

        private IEnumerator SendStatusUpdateLoop()
        {
            while (networkStore == null || serverAPI == null)
            {
                networkStore = EnsureStore();
                serverAPI = EnsureServerAPI();
                yield return null;
            }

            int updateInterval = serverAPI.User.StatusUpdateInterval;

            while (enableStatusUpdate)
            {
                yield return new WaitForSeconds(updateInterval);

                string login = networkStore.MyPlayerInfo.Login;
                if (!string.IsNullOrEmpty(login))
                {
                    Task request = serverAPI.User.UpdateMyServerStatus();

                    yield return new WaitUntil(() => request.IsCompleted);
                }
            }
        }

        private NetworkDataStore EnsureStore()
        {
            if (networkStore == null && MVNetworkManager.singleton != null)
            {
                return MVNetworkManager.singleton.NetworkStore;
            }

            return networkStore;
        }

        private APIContainer EnsureServerAPI()
        {
            if (serverAPI == null && MVNetworkManager.singleton != null)
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer;
            }

            return serverAPI;
        }
    }
}