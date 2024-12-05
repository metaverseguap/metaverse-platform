using System.Collections;
using Global.Logger;
using Mirror;
using NetworkCore.MirrorNetworking.Player;
using NetworkCore.MirrorNetworking.Player.TempPlayer;
using NetworkCore.MirrorNetworking.Types.HostMigration;
using NetworkCore.ServerInteraction.API;
using NetworkCore.Utils;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Hosts.Migration
{
    /// <summary>
    /// <para>Функциональность миграции хоста.</para>
    ///
    /// <remarks>Компонент предоставляет внешнюю обработку <see cref="MVNetworkManager"/></remarks>
    /// </summary>
    public sealed class HostMigration : MonoBehaviour
    {
        #region Singleton
        private static HostMigration instance = null;

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
        
        private MVNetworkManager networkManager;
        private HostMigrationState migrationState;
        private APIContainer serverAPI;
        
        private void Start()
        {
            networkManager = MVNetworkManager.singleton;
            migrationState = networkManager.NetworkStore.MigrationState;
            serverAPI = networkManager.NetworkStore.FileServer;
            
            networkManager.AfterServerAddPlayer += OnServerAddPlayer;
            networkManager.BeforeClientDisconnected += OnClientDisconnect;
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.AfterServerAddPlayer -= OnServerAddPlayer;
                networkManager.BeforeClientDisconnected -= OnClientDisconnect;
            }
        }
        
        private void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            SetBackUpHost();
        }

        private void OnClientDisconnect()
        {
            if (migrationState.PlayerMigrationStatus != HostMigrationStatus.I_AM_SERVER)
            {
                StartCoroutine(HostMigrate());
            }
            else
            {
                //serverAPI.RemoveHost();
            }
        }
        
        private void SetBackUpHost()
        {
            NetworkConnectionToClient nextHost = GetNextHost();

            if (nextHost == null) return;

            //once found send to each client to store
            NetworkGamePlayer newHost = nextHost.identity.GetComponent<NetworkGamePlayer>();
            newHost.StoreNewHostData(nextHost.identity.netId);
        }
        
        private NetworkConnectionToClient GetNextHost()
        {
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity.isLocalPlayer) continue;

                return conn;
            }

            return null;
        }

        private IEnumerator HostMigrate()
        {
            if (migrationState.PlayerMigrationStatus == HostMigrationStatus.I_AM_NEW_HOST)
            {
                //these delays can be played with,
                //i was told we have to wait x amount of frames
                //before attempting to start
                yield return new WaitForSeconds(0.3f);
                string url = IPUtils.UrlFromIP(migrationState.NewHostCache.IP);
                //serverAPI.CreateHost(url, HostMigrationState.HostCache.SceneName);
                networkManager.StartHost();
                AppLogger.Log("New host");
            }
            else
            {
                //these delays can be played with,
                //i was told we have to wait x amount of frames
                //before attempting to start
                yield return new WaitForSeconds(0.6f);

                networkManager.networkAddress = migrationState.NewHostCache.IP;
                networkManager.StartClient();
                AppLogger.Log("New client");
            }

            yield return null;
        }
    }
}