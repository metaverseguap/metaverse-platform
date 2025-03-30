using Mirror;
using NetworkCore.MirrorNetworking.ClientMessages;
using NetworkCore.MirrorNetworking.Containers.ClientMessages;
using NetworkCore.MirrorNetworking.Containers.Store;
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
            }
        }
        
        private void OnClientStarted()
        {
        }
        
        private void OnHostStarted()
        {
            OnServerAddPlayer(NetworkServer.localConnection);
            
            // Регистрация клиентских сообщений
            // Клиент не имеет прямого доступа к серверным методам и объектам.
            // Что бы запросить у сервера вызвать какой-либо метод у себя, используются сообщения `NetworkMessage`.
            // Сообщения регистрируются на сервере вызовом метода `NetworkServer#RegisterHandler`
            NetworkServer.RegisterHandler<ClientRegistrationMessage>(OnClientConnectedToServer);
        }

        private void OnClientConnectedToServer(NetworkConnectionToClient conn, ClientRegistrationMessage message)
        {
            OnServerAddPlayer(conn, message.SpawnData);
        }

        private void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            OnServerAddPlayer(conn, null);
        }

        private void OnServerAddPlayer(NetworkConnectionToClient conn, ClientSpawnData clientData)
        {
            NetworkBasePlayer networkPlayer = CreateNetworkPlayer(clientData);
            
            MVNetworkManager.singleton.NetworkStore.GamePlayers.Add(conn.connectionId, networkPlayer);
            
            AddClientToServer(conn, networkPlayer);
        }

        private NetworkBasePlayer CreateNetworkPlayer(ClientSpawnData clientData)
        {
            NetworkBasePlayer networkPlayer = Instantiate(networkStore.Player.NetworkPlayer);

            string displayName;
            string avatarName;
            if (clientData != null)
            {
                displayName = clientData.Nickname;
                avatarName = clientData.Avatar;
            }
            else
            {
                UserInfo userInfo = networkStore.FileServer.User.GetMyUser();
                displayName = userInfo.Nickname;
                avatarName = networkStore.Player.AvatarName;
            }
            
            networkPlayer.SetDisplayName(displayName);
            if(networkPlayer is NetworkAvatarPlayer avatarPlayer)
            {
                avatarPlayer.SetAvatarName(avatarName);
            }
            
            NetworkServer.Spawn(networkPlayer.gameObject);
    
            return networkPlayer;
        }
        
        private static void AddClientToServer(NetworkConnectionToClient conn, NetworkBasePlayer networkPlayer)
        {
            if (!NetworkClient.ready)
            {
                NetworkClient.Ready();
            }

            if (NetworkClient.ready)
            {
                NetworkServer.AddPlayerForConnection(conn, networkPlayer.gameObject);
            }
        }
        
        private void OnServerLostPlayer(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                MVNetworkManager.singleton.NetworkStore.GamePlayers.Remove(conn.connectionId);
            }
        }
        
        private void OnServerStop()
        {
            MVNetworkManager.singleton.NetworkStore.GamePlayers.Clear();
        }
        
        private void OnClientConnected()
        {
            SendRegistrationMessageToServer();
        }
        
        private void SendRegistrationMessageToServer()
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
                ClientSpawnData clientSpawnData = new ClientSpawnData();
                UserInfo userInfo = networkStore.FileServer.User.GetMyUser();
                clientSpawnData.Nickname = userInfo.Nickname;
                clientSpawnData.Avatar = networkStore.Player.AvatarName;
                
                // Клиент не имеет прямого доступа к серверным методам и объектам.
                // Что бы запросить у сервера вызвать какой-либо метод у себя, используются сообщения NetworkMessage.
                // Сообщения отправляются на сервер при помощи метода `NetworkClient#Send`
                NetworkClient.Send(new ClientRegistrationMessage(clientSpawnData));
            }
        }

        private void OnClientDisconnected()
        {
        }
    }
}