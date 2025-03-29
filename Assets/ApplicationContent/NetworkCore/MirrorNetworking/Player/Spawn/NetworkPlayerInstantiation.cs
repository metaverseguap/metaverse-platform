using Global.Logger;
using Mirror;
using NetworkCore.MirrorNetworking.ClientMessages;
using NetworkCore.MirrorNetworking.Containers;
using NetworkCore.MirrorNetworking.Player.AvatarPlayer;
using NetworkCore.MirrorNetworking.Player.Base;
using UnityEngine;
using UserSystem.Types;

namespace NetworkCore.MirrorNetworking.Player.Spawn
{
    /// <summary>
    /// <para>Логика создания игрока на сервере.</para>
    /// </summary>
    public sealed class NetworkPlayerInstantiation : MonoBehaviour
    {
        #region Singleton

        private static NetworkPlayerInstantiation instance = null;

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
        private NetworkDataStore networkStore;

        private void Start()
        {
            networkManager = MVNetworkManager.singleton;
            networkStore = networkManager.NetworkStore;

            // События хоста
            networkManager.AfterStartServerOrHost += OnHostStarted;
            networkManager.AfterStartClient += OnClientStarted;
            networkManager.AfterServerAddPlayer += OnServerAddPlayer;
            
            networkManager.BeforeServerLostPlayer += OnServerLostPlayer;
            networkManager.BeforeServerStop += OnServerStop;
            
            // События клиента
            networkManager.AfterClientConnected += OnClientConnected;
            networkManager.BeforeClientDisconnected += OnClientDisconnected;
            
            // События сцен
            networkManager.AfterServerChangeScene += OnServerChangeScene;
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                // События хоста
                networkManager.BeforeClientDisconnected -= OnHostStarted;
                networkManager.AfterStartClient -= OnClientStarted;
                networkManager.AfterServerAddPlayer -= OnServerAddPlayer;
                
                networkManager.BeforeServerLostPlayer -= OnServerLostPlayer;
                networkManager.BeforeServerStop -= OnServerStop;
                
                // События клиента
                networkManager.AfterClientConnected -= OnClientConnected;
                networkManager.BeforeClientDisconnected -= OnClientDisconnected;
                
                // События сцен
                networkManager.AfterServerChangeScene -= OnServerChangeScene;
            }
        }
        
        private void OnServerChangeScene(string newSceneName)
        {
            
        }

        private void OnHostStarted()
        {
            AppLogger.Log("Host Started");
            
            OnServerAddPlayer(NetworkServer.localConnection);
            NetworkServer.RegisterHandler<ClientRegistrationMessage>(OnAddPlayer);
        }

        private void OnAddPlayer(NetworkConnectionToClient conn, ClientRegistrationMessage message)
        {
            AppLogger.Log("Adding player from client request");
            OnServerAddPlayer(conn);
        }


        private void OnClientStarted()
        {
            AppLogger.Log("Client Started");
        }

        private void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            AppLogger.Warning("Server Add Player");
            
            NetworkBasePlayer networkPlayer = CreateNetworkPlayer();
            
            MVNetworkManager.singleton.NetworkStore.GamePlayers.Add(conn.connectionId, networkPlayer);
            
            if (!NetworkClient.ready)
            {
                NetworkClient.Ready();
            }

            if (NetworkClient.ready)
            {
                NetworkServer.AddPlayerForConnection(conn, networkPlayer.gameObject);
            }
        }

        private NetworkBasePlayer CreateNetworkPlayer()
        {
            NetworkBasePlayer networkPlayerPrefab = networkStore.Player.NetworkPlayer;
            NetworkBasePlayer networkPlayer = Instantiate(networkPlayerPrefab);
            

            UserInfo userInfo = networkStore.FileServer.User.GetMyUser();
            networkPlayer.SetDisplayName(userInfo.Nickname);
            if(networkPlayer is NetworkAvatarPlayer avatarPlayer)
            {
                avatarPlayer.SetAvatarName(networkStore.Player.AvatarName);
            }
            
            
            NetworkServer.Spawn(networkPlayer.gameObject);
    
            return networkPlayer;
        }
        
        private void OnServerLostPlayer(NetworkConnectionToClient conn)
        {
            AppLogger.Log("Server Lost Player");
            
            if (conn.identity != null)
            {
                MVNetworkManager.singleton.NetworkStore.GamePlayers.Remove(conn.connectionId);
            }
        }
        
        private void OnServerStop()
        {
            AppLogger.Log("Server Stop");
            
            MVNetworkManager.singleton.NetworkStore.GamePlayers.Clear();
        }
        
        private void OnClientConnected()
        {
            AppLogger.Log("Client Connected");
            
            SendRegistrationMessageToServer();
        }
        
        /// Клиент не имеет прямого доступа к серверным методам и объектам.
        /// Что бы запросить у сервера вызвать какой-либо метод у себя, используются сообщения <c>NetworkMessage</c>.
        /// Сообщения регистрируются на сервере вызовом метода <c>NetworkServer.RegisterHandler</c>.
        /// Сообщения отправляются на сервер при помощи метода <c>NetworkClient.Send</c>.
        private static void SendRegistrationMessageToServer()
        {
            if (NetworkServer.active)
            {
                return;
            }
            
            if (!NetworkClient.ready)
            {
                NetworkClient.Ready();
            }
            
            if (NetworkClient.isConnected)
            {
                NetworkClient.Send(new ClientRegistrationMessage());
            }
        }

        private void OnClientDisconnected()
        {
            AppLogger.Log("Client Disconnected");
        }
    }
}