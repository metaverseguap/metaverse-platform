using System.Collections;
using Global.Logger;
using Mirror;
using NetworkCore.MirrorNetworking.Player;
using NetworkCore.MirrorNetworking.Types;
using NetworkCore.ServerInteraction;
using NetworkCore.ServerInteraction.API;
using NetworkCore.Utils;
using RoleSystem.Types;
using UnityEngine;

namespace NetworkCore.MirrorNetworking
{
    /// <summary>
    /// <para>Класс идентифицирующий локальную машину в сети.</para>
    ///
    /// Данный класс служит для подключения локальной машины к сети mirror,
    /// либо для создания из локальной машины хоста в сети mirror.
    ///
    /// Данный класс так же идентифицирует локальную машину в сети mirror. 
    /// </summary>
    public sealed class MVNetworkManager : NetworkManager
    {
        [Header("Server")] 
        [SerializeField] private string _serverUri;

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.singleton"/></para>
        /// <para>Доступ к данному типу NetworkManager.</para>
        /// 
        /// Данное свойство стоит использовать для получения экземпляра MVNetworkManager в других классах
        /// </summary>
        public new static MVNetworkManager singleton { get; private set; }

        /// <summary>
        /// <para>Контейнер состояния игрока при миграции хоста.</para>
        /// </summary>
        public HostMigrationState HostMigrationState { get; set; } = new HostMigrationState();

        /// <summary>
        /// <para>Доступ к файловму серверу.</para>
        /// </summary>
        public APIContainer FileServer { get; private set; }

        /// <summary>
        /// <para>Роль пользователя.</para>
        /// </summary>
        public RoleInfo Role { get; set; }

        public override void Awake()
        {
            base.Awake();
            singleton = this;
            FileServer = new APIContainer(_serverUri);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            SetBackUpHost();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
        }

        public override void OnClientDisconnect()
        {
            if (HostMigrationState.PlayerMigrationStatus != HostMigrationStatus.I_AM_SERVER)
            {
                StartCoroutine(HostMigrate());
            }
            else
            {
                //serverAPI.RemoveHost();
            }

            base.OnClientDisconnect();
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
            if (HostMigrationState.PlayerMigrationStatus == HostMigrationStatus.I_AM_NEW_HOST)
            {
                //these delays can be played with,
                //i was told we have to wait x amount of frames
                //before attempting to start
                yield return new WaitForSeconds(0.3f);
                string url = IPUtils.UrlFromIP(HostMigrationState.NewHostCache.IP);
                //serverAPI.CreateHost(url, HostMigrationState.HostCache.SceneName);
                StartHost();
                AppLogger.Log("New host");
            }
            else
            {
                //these delays can be played with,
                //i was told we have to wait x amount of frames
                //before attempting to start
                yield return new WaitForSeconds(0.6f);

                networkAddress = HostMigrationState.NewHostCache.IP;
                StartClient();
                AppLogger.Log("New client");
            }

            yield return null;
        }
    }
}