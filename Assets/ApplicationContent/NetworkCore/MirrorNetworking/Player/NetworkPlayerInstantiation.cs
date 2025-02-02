using AppAvatars;
using AppAvatars.Containers;
using MainMenu.Containers;
using Mirror;
using NetworkCore.MirrorNetworking.Containers;
using NetworkCore.MirrorNetworking.Player.Base;
using NetworkCore.ServerInteraction.API;
using UnityEngine;
using UserSystem.Types;

namespace NetworkCore.MirrorNetworking.Player
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
        private APIContainer serverAPI;

        private void Start()
        {
            networkManager = MVNetworkManager.singleton;
            networkStore = networkManager.NetworkStore;
            serverAPI = networkStore.FileServer;

            networkManager.AfterServerAddPlayer += OnServerAddPlayer;
            networkManager.AfterHostStarted += OnHostStarted;
            networkManager.AfterServerLostPlayer += OnServerLostPlayer;
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.AfterServerAddPlayer -= OnServerAddPlayer;
                networkManager.AfterHostStarted -= OnHostStarted;
                networkManager.AfterServerLostPlayer -= OnServerLostPlayer;
            }
        }

        private void OnHostStarted()
        {
            if (NetworkServer.localConnection != null)
            {
                OnServerAddPlayer(NetworkServer.localConnection);
            }
        }

        private void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            NetworkBasePlayer newPrefab = networkStore.Player.NetworkPlayer;

            var basePlayerInstance = Instantiate(newPrefab);
            MVNetworkManager.singleton.NetworkStore.GamePlayers.Add(conn.connectionId, basePlayerInstance);

            if (conn == NetworkServer.localConnection)
            {
                SetupNetworkPlayer(conn, basePlayerInstance);
            }

            CreatePlayerAvatar(conn, basePlayerInstance);

            if (NetworkClient.Ready())
            {
                NetworkServer.AddPlayerForConnection(conn, basePlayerInstance.gameObject);
            }
        }

        private void SetupNetworkPlayer(NetworkConnectionToClient conn, NetworkBasePlayer basePlayerInstance)
        {
            DontDestroyOnLoad(basePlayerInstance);

            UserInfo userInfo = serverAPI.User.GetMyUser();
            basePlayerInstance.SetDisplayName(userInfo.Nickname);
            basePlayerInstance.SetAvatarName(networkStore.Player.AvatarName);
            basePlayerInstance.SetConnectedId(conn.connectionId);
        }

        private void CreatePlayerAvatar(NetworkConnectionToClient conn, NetworkBasePlayer basePlayerInstance)
        {
            if (networkStore.GamePlayers.TryGetValue(conn.connectionId, out NetworkBasePlayer gamePlayer))
            {
                string avatarName = gamePlayer.AvatarName;

                AvatarFile avatar = networkStore.Avatars.AvatarFiles[avatarName];

                AbstractPlayer playerController = Instantiate(networkStore.Player.CurrentBuildPlayer);

                AvatarPrefabInfo avatarPrefabInfo = new AvatarPrefabInfo();
                avatarPrefabInfo.Prefab = avatar.Model.GetComponent<Animator>();
                avatarPrefabInfo.ForGender = avatar.AvatarGender;
                playerController.AvatarComponent.CreatePlayerFromAvatar(avatarPrefabInfo);

                basePlayerInstance.PlayerController = playerController;

                if (conn == NetworkServer.localConnection)
                {
                    DontDestroyOnLoad(playerController.gameObject);
                }
            }
        }

        private void OnServerLostPlayer(NetworkConnectionToClient conn)
        {
            NetworkBasePlayer networkPlayer = MVNetworkManager.singleton.NetworkStore.GamePlayers[conn.connectionId];
            Destroy(networkPlayer.PlayerController.gameObject);

            NetworkServer.DestroyPlayerForConnection(conn);
            MVNetworkManager.singleton.NetworkStore.GamePlayers.Remove(conn.connectionId);
        }
    }
}