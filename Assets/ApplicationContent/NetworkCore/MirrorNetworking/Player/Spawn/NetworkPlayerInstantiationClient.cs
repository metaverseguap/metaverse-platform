using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using LDR.SUAI_Metaverse.SDK.SceneLogic.SpawnPoint;
using LDR.SUAI_Metaverse.SDK.Utils;
using Mirror;
using NetworkCore.MirrorNetworking.ClientMessages;
using NetworkCore.MirrorNetworking.Containers.ClientMessages;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Containers.Store.Cache;
using NetworkCore.MirrorNetworking.Containers.Synchronization;
using NetworkCore.MirrorNetworking.Player.Base;
using NetworkCore.MirrorNetworking.Synchronization.Animations;
using NetworkCore.MirrorNetworking.Synchronization.Transforms;
using NetworkCore.MirrorNetworking.Synchronization.UI;
using NetworkCore.MirrorNetworking.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserSystem.Types;

namespace NetworkCore.MirrorNetworking.Player.Spawn
{
    /// <summary>
    /// <para>Логика создания игрока на клиенте.</para>
    /// </summary>
    public sealed class NetworkPlayerInstantiationClient : MonoBehaviour
    {
        #region Singleton

        private static NetworkPlayerInstantiationClient instance = null;

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
        
        private const float HOST_MIGRATION_TIME = 0.3f;
        
        private MVNetworkManager networkManager;

        private void Start()
        {
            networkManager = MVNetworkManager.singleton;
            
            networkManager.AfterClientConnected += OnClientConnected;
            networkManager.AfterStartClient += OnClientStarted;
            networkManager.BeforeClientDisconnected += OnClientDisconnected;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadingMode)
        {
            if (loadingMode == LoadSceneMode.Single)
            {
                SceneSpawner sceneSpawner = SceneSpawner.singleton;
                sceneSpawner.IsOfflineSpawnActive = false;
            }
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.AfterClientConnected -= OnClientConnected;
                networkManager.AfterStartClient -= OnClientStarted;
                networkManager.BeforeClientDisconnected -= OnClientDisconnected;
            }
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
                var networkStore = MVNetworkManager.singleton.NetworkStore;

                var clientSpawnData = CreateClientSpawnData(networkStore);

                // Клиент не имеет прямого доступа к серверным методам и объектам.
                // Что бы запросить у сервера вызвать какой-либо метод у себя, используются сообщения NetworkMessage.
                // Сообщения отправляются на сервер при помощи метода `NetworkClient#Send`
                NetworkClient.Send(new ClientRegistrationMessage(clientSpawnData));
            }
        }

        private static ClientSpawnData CreateClientSpawnData(NetworkDataStore networkStore)
        {
            ClientSpawnData clientSpawnData = new ClientSpawnData();

            UserInfo userInfo = networkStore.FileServer.User.GetMyUser();
            clientSpawnData.Login = userInfo.Login;
            clientSpawnData.Nickname = userInfo.Nickname;
            clientSpawnData.Avatar = networkStore.MyPlayerInfo.AvatarName;

            PlayerCache cache = networkStore.HostMigration.Caches.CurrentPlayerCache;

            clientSpawnData.IsTransformCached = cache.IsInitialized;
            clientSpawnData.PlayerPosition = cache.PlayerPosition;
            clientSpawnData.PlayerRotation = cache.PlayerRotation;

            if (cache.PlayerCameraRotations != null)
            {
                clientSpawnData.PlayerCameraRotations = cache.PlayerCameraRotations.Clone() as NamedTransform[];
            }
            else
            {
                clientSpawnData.PlayerCameraRotations = Array.Empty<NamedTransform>();
            }

            return clientSpawnData;
        }
        
        private void OnClientStarted()
        {
            // Регистрация серверных сообщений для клиента
            NetworkClient.RegisterHandler<RoomStateMessage>(SetRoomState);
            NetworkClient.RegisterHandler<BackupHostMessage>(SetBackupHost);
            NetworkClient.RegisterHandler<NewPlayerConnectedMessage>(AddGamePlayerToRoomState);
            NetworkClient.RegisterHandler<PlayerDisconnectedMessage>(RemovePlayerFromRoomState);
        }

        private void SetRoomState(RoomStateMessage message)
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;
            networkStore.RoomState.Clear();
            networkStore.RoomState.RoomStartTime = new DateTime(message.RoomStartTimeTicks, DateTimeKind.Utc);
            foreach (uint netId in message.GamePlayersNetIds)
            {
                StartCoroutine(AddPlayerWithNetIdToRoomState(netId));
            }

            networkStore.RoomState.Initialized = message.Initialized;
            networkStore.NetworkProvider.NotifyRoomConnection();
        }

        private void SetBackupHost(BackupHostMessage message)
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;
            networkStore.HostMigration.BackupHostLogin = message.BackupHostLogin;
        }
        
        private void AddGamePlayerToRoomState(NewPlayerConnectedMessage message)
        {
            StartCoroutine(AddPlayerWithNetIdToRoomState(message.netId));
        }

        private void RemovePlayerFromRoomState(PlayerDisconnectedMessage message)
        {
            NetworkDataStore networkStore = MVNetworkManager.singleton.NetworkStore;
            networkStore.RoomState.GamePlayers.Remove(message.netId);
        }

        private IEnumerator AddPlayerWithNetIdToRoomState(uint netId)
        {
            // Ждем 1 кадр, что бы игрок успел заспавниться
            yield return null;

            NetworkDataStore networkStore = MVNetworkManager.singleton.NetworkStore;
            NetworkBasePlayer player = null;
            while (player == null)
            {
                if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
                {
                    player = identity.GetComponent<NetworkBasePlayer>();
                    networkStore.RoomState.GamePlayers[player.netId] = player;
                }

                yield return null;
            }
        }

        private void OnClientDisconnected()
        {
            NetworkDataStore networkStore = MVNetworkManager.singleton.NetworkStore;

            if (NetworkClient.connection != null && NetworkClient.connection.identity != null
                && networkStore.HostMigration.BackupHostLogin != null)
            {
                NetworkBasePlayer player = NetworkClient.connection.identity.GetComponent<NetworkBasePlayer>();
                if (player.isLocalPlayer)
                {
                    PlayerCache playerCache = networkStore.HostMigration.Caches.CurrentPlayerCache;
                    CachePlayerTransform(player, ref playerCache);

                    if (networkStore.HostMigration.BackupHostLogin == player.Login)
                    {
                        CacheSyncObjectsTransform(ref networkStore);
                        CacheSyncUIObjects(ref networkStore);
                        CacheSyncAnimations(ref networkStore);
                    }
                }

                StartCoroutine(HostMigrate());
            }
            else
            {
                ClearRoomState();
            }
        }

        private static void CachePlayerTransform(NetworkBasePlayer player, ref PlayerCache cache)
        {
            NetworkTransformReliable playerTransformSync = player.GetComponent<NetworkTransformReliable>();
            Transform playerTransform = playerTransformSync.target;

            cache.IsInitialized = true;
            cache.PlayerPosition = playerTransform.position;
            cache.PlayerRotation = playerTransform.rotation;

            CachePlayerCameraRotation(player, ref cache);
        }

        private static void CachePlayerCameraRotation(NetworkBasePlayer player, ref PlayerCache cache)
        {
            CinemachineVirtualCamera[] playerCameras = player.GetComponentsInChildren<CinemachineVirtualCamera>(true);
            List<NamedTransform> cameraRotations = new List<NamedTransform>();
            foreach (var virtualCamera in playerCameras)
            {
                GameObject cameraGameObject = virtualCamera.gameObject;
                
                string objAbsolutePath = SceneUtils.GetObjectPathFromRoot(cameraGameObject.transform, player.transform);
                
                NamedTransform namedTransform = new NamedTransform(objAbsolutePath, cameraGameObject.transform.rotation);

                cameraRotations.Add(namedTransform);
            }

            cache.PlayerCameraRotations = cameraRotations.ToArray();
        }

        private static void CacheSyncObjectsTransform(ref NetworkDataStore networkStore)
        {
            MVBaseNetworkTransform[] syncTransforms = FindObjectsByType<MVBaseNetworkTransform>(FindObjectsSortMode.None);

            foreach (var syncTransform in syncTransforms)
            {
                var syncObject = syncTransform.gameObject;
                var position = syncObject.transform.position;
                var rotation = syncObject.transform.rotation;

                networkStore.HostMigration.Caches.CurrentSceneCache.SyncTransformsCache[syncTransform.netIdentity.sceneId] = new NamedTransform(syncTransform.netIdentity.sceneId.ToString(), position, rotation);
            }
        }

        private void CacheSyncUIObjects(ref NetworkDataStore networkStore)
        {
            MVNetworkUI[] syncUIs = FindObjectsByType<MVNetworkUI>(FindObjectsSortMode.None);
            foreach (var syncUI in syncUIs)
            {
                networkStore.HostMigration.Caches.CurrentSceneCache.SyncUITextsCache[syncUI.netIdentity.sceneId] = syncUI.TextsContent.Clone() as string[];
            }
        }

        private void CacheSyncAnimations(ref NetworkDataStore networkStore)
        {
            MVNetworkAnimator[] syncAnimators = FindObjectsByType<MVNetworkAnimator>(FindObjectsSortMode.None);
            foreach (var animator in syncAnimators)
            {
                networkStore.HostMigration.Caches.CurrentSceneCache.SyncAnimationVariableCache[animator.netIdentity.sceneId] = new Dictionary<string, object>(animator.CurrentVariables);
                if (animator.ExternalAnimationsIds != null)
                {
                    networkStore.HostMigration.Caches.CurrentSceneCache.SyncExternalAnimationIdsCache[animator.netIdentity.sceneId] =
                        new Tuple<string, List<string>>(
                            animator.ExternalAnimationsIds.Item1,
                            animator.ExternalAnimationsIds.Item2
                        );
                }
            }
        }

        private IEnumerator HostMigrate()
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;

            bool amINewHost = networkStore.HostMigration.BackupHostLogin == networkStore.MyPlayerInfo.Login;
            if (amINewHost)
            {
                // Для корректной работы необходима задержка в несколько кадров перед началом миграции
                yield return new WaitForSeconds(HOST_MIGRATION_TIME);
                networkManager.BecomeHostImmediate(networkStore.Connection.CurrentScene);
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    // Для корректной работы необходима задержка в несколько кадров перед началом миграции
                    yield return new WaitForSeconds(2 * HOST_MIGRATION_TIME);

                    bool becomeAClient = networkManager.TryToBecomeAClientImmediate(networkStore.HostMigration.BackupHostLogin, networkStore.Connection.CurrentScene);
                    if (becomeAClient)
                    {
                        break;
                    }
                }
            }

            yield return null;
        }

        private void ClearRoomState()
        {
            var networkStore = MVNetworkManager.singleton.NetworkStore;
            networkStore.RoomState.Clear();
            networkStore.HostMigration.Clear();
        }
    }
}