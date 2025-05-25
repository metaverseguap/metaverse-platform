using System;
using System.Linq;
using Global.Logger;
using LDR.SUAI_Metaverse.SDK.Core.SceneLogic.SpawnPoint;
using Mirror;
using NetworkCore.MirrorNetworking.ClientMessages;
using NetworkCore.MirrorNetworking.Containers.ClientMessages;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Containers.Store.Cache;
using NetworkCore.MirrorNetworking.Containers.Synchronization;
using NetworkCore.MirrorNetworking.Player.AvatarPlayer;
using NetworkCore.MirrorNetworking.Player.Base;
using NetworkCore.MirrorNetworking.Synchronization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserSystem.Types;
using Random = UnityEngine.Random;

namespace NetworkCore.MirrorNetworking.Player.Spawn
{
    /// <summary>
    /// <para>Логика создания игрока на сервере.</para>
    /// </summary>
    [RequireComponent(typeof(NetworkPlayerInstantiationClient))]
    public sealed class NetworkPlayerInstantiationServer : MonoBehaviour
    {
        #region Singleton

        private static NetworkPlayerInstantiationServer instance = null;

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

        private SceneSpawner sceneSpawner = null;
        
        private void Start()
        {
            networkManager = MVNetworkManager.singleton;

            networkManager.AfterStartServerOrHost += OnHostStarted;
            networkManager.AfterServerAddPlayer += OnServerAddPlayer;
            networkManager.AfterNewClientConnectedToServer += OnNewClientConnectedToServer;
            networkManager.BeforeServerLostPlayer += OnServerLostPlayer;
            networkManager.BeforeServerStop += OnServerStop;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadingMode)
        {
            if (loadingMode == LoadSceneMode.Single)
            {
                sceneSpawner = SceneSpawner.singleton;
                sceneSpawner.IsOfflineSpawnActive = false;
            }
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.BeforeClientDisconnected -= OnHostStarted;
                networkManager.AfterServerAddPlayer -= OnServerAddPlayer;
                networkManager.AfterNewClientConnectedToServer -= OnNewClientConnectedToServer;
                networkManager.BeforeServerLostPlayer -= OnServerLostPlayer;
                networkManager.BeforeServerStop -= OnServerStop;
            }
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnHostStarted()
        {
            OnServerAddPlayer(NetworkServer.localConnection);

            // Регистрация клиентских сообщений
            // Клиент не имеет прямого доступа к серверным методам и объектам.
            // Что бы запросить у сервера вызвать какой-либо метод у себя, используются сообщения `NetworkMessage`.
            // Сообщения регистрируются на сервере вызовом метода `NetworkServer#RegisterHandler`
            NetworkServer.RegisterHandler<ClientRegistrationMessage>(OnClientConnectedToServer);

            InitRoomState();
        }

        private void InitRoomState()
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;
            networkStore.RoomState.RoomStartTime = DateTime.UtcNow;
            networkStore.RoomState.Initialized = true;
            networkStore.NetworkProvider.NotifyRoomConnection();
        }
        
        private void OnNewClientConnectedToServer(NetworkConnectionToClient conn)
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;
            // Отправляем текущее состояние комнаты хоста подключившемуся клиенту
            RoomStateStore currentRoomState = networkStore.RoomState;
            bool initialized = currentRoomState.Initialized;
            long roomStartTimeTicks = currentRoomState.RoomStartTime.Ticks;
            uint[] gamePlayersNetIds = currentRoomState.GamePlayers.Keys.ToArray();
            var roomStateMessage = new RoomStateMessage(initialized, roomStartTimeTicks, gamePlayersNetIds);
            conn.Send(roomStateMessage);
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

            AddClientToServer(conn, networkPlayer);

            MVNetworkManager.singleton.NetworkStore.RoomState.GamePlayers[conn.identity.netId] = networkPlayer;
            
            SendPlayerIdToClients(conn);
            SetupNewBackupHost();
        }

        private static void SendPlayerIdToClients(NetworkConnectionToClient conn)
        {
            var message = new NewPlayerConnectedMessage(conn.identity.netId);
            SendMessageToAllClients(message);
        }

        private void SetupNewBackupHost(NetworkConnectionToClient excludeConnection = null)
        {
            NetworkDataStore networkStore = MVNetworkManager.singleton.NetworkStore;

            NetworkConnectionToClient newHostConnection = GetNextHost(excludeConnection);
            if (newHostConnection == null)
            {
                MVNetworkManager.singleton.NetworkStore.HostMigration.BackupHostLogin = null;
            }
            else
            {
                NetworkBasePlayer newHost = MVNetworkManager.singleton.NetworkStore.RoomState.GamePlayers[newHostConnection.identity.netId];

                networkStore.HostMigration.BackupHostLogin = newHost.Login;
            }

            var message = new BackupHostMessage(networkStore.HostMigration.BackupHostLogin);
            SendMessageToAllClients(message);
        }

        private static void SendMessageToAllClients<T>(T message)
            where T : struct, NetworkMessage
        {
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity == null || conn.identity.isLocalPlayer)
                {
                    continue;
                }

                conn.Send(message);
            }
        }

        private NetworkConnectionToClient GetNextHost(NetworkConnectionToClient excludeConnection)
        {
            // TODO: Придумать алгоритм выбора наилучшего хоста (ME-53)
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity == null)
                {
                    AppLogger.Log($"Server connection {conn.connectionId} is null");
                    continue;
                }

                if (conn.identity.isLocalPlayer || conn == excludeConnection)
                {
                    continue;
                }

                AppLogger.Log($"New backup host connection is {conn.connectionId}");

                return conn;
            }

            return null;
        }

        private NetworkBasePlayer CreateNetworkPlayer(ClientSpawnData clientData)
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;
            NetworkBasePlayer networkPlayer = Instantiate(networkStore.Configuration.SpawnablePrefabs.NetworkPlayer);

            string login;
            string displayName;
            string avatarName;
            PlayerCache playerCache = new PlayerCache();
            if (clientData != null)
            {
                login = clientData.Login;
                displayName = clientData.Nickname;
                avatarName = clientData.Avatar;

                playerCache.IsInitialized = clientData.IsTransformCached;
                playerCache.PlayerPosition = clientData.PlayerPosition;
                playerCache.PlayerRotation = clientData.PlayerRotation;
                playerCache.PlayerCameraRotations = clientData.PlayerCameraRotations.Clone() as NamedTransform[];
            }
            else
            {
                UserInfo userInfo = networkStore.FileServer.User.GetMyUser();
                login = userInfo.Login;
                displayName = userInfo.Nickname;
                avatarName = networkStore.MyPlayerInfo.AvatarName;
                var cache = networkStore.HostMigration.Caches.CurrentPlayerCache;

                playerCache.IsInitialized = cache.IsInitialized;
                playerCache.PlayerPosition = cache.PlayerPosition;
                playerCache.PlayerRotation = cache.PlayerRotation;
                if (cache.PlayerCameraRotations != null)
                {
                    playerCache.PlayerCameraRotations = cache.PlayerCameraRotations.Clone() as NamedTransform[];
                }
                else
                {
                    playerCache.PlayerCameraRotations = Array.Empty<NamedTransform>();
                }
            }

            SpawnPoint[] spawnPoints = sceneSpawner.SpawnPoints();
            if (!playerCache.IsInitialized && spawnPoints.Length > 0)
            {
                playerCache.IsInitialized = true;
                Transform spawnPoint = GetSpawnPointTransform(spawnPoints);
                playerCache.PlayerPosition = spawnPoint.position;
                playerCache.PlayerRotation = spawnPoint.rotation;
            }

            networkPlayer.SetLogin(login);
            networkPlayer.SetDisplayName(displayName);
            if (networkPlayer is NetworkAvatarPlayer avatarPlayer)
            {
                avatarPlayer.SetAvatarName(avatarName);
                avatarPlayer.SetCache(playerCache);
            }

            NetworkServer.Spawn(networkPlayer.gameObject);

            return networkPlayer;
        }

        private static Transform GetSpawnPointTransform(SpawnPoint[] spawnPoints)
        {
            SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return spawnPoint.transform;
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
            NetworkDataStore networkStore = MVNetworkManager.singleton.NetworkStore;
            RoomStateStore roomState = networkStore.RoomState;
            if (conn.identity != null && roomState.Initialized)
            {
                MVNetworkInteractionAccess[] accesses = FindObjectsByType<MVNetworkInteractionAccess>(FindObjectsSortMode.None);
                foreach (var access in accesses)
                {
                    access.ReleaseControlFrom(conn);
                }

                NetworkBasePlayer lostPlayer = roomState.GamePlayers[conn.identity.netId];

                if (lostPlayer.Login == networkStore.HostMigration.BackupHostLogin)
                {
                    SetupNewBackupHost(conn);
                }

                MVNetworkManager.singleton.NetworkStore.RoomState.GamePlayers.Remove(conn.identity.netId);

                RemovePlayerIdFromClients(conn);
            }
        }

        private static void RemovePlayerIdFromClients(NetworkConnectionToClient conn)
        {
            var message = new PlayerDisconnectedMessage(conn.identity.netId);
            SendMessageToAllClients(message);
        }

        private void OnServerStop()
        {
            ClearRoomState();
        }

        private void ClearRoomState()
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;
            networkStore.RoomState.Clear();
            networkStore.HostMigration.Clear();
        }
    }
}