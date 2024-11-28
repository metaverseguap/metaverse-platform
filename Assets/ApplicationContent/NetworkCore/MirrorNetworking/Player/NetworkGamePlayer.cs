using Mirror;
using NetworkCore.MirrorNetworking.Types;
using NetworkCore.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkCore.MirrorNetworking.Player
{
    /// <summary>
    /// <para>Сетевое представление игрока.</para>
    /// </summary>
    /// TODO: При создании сетевой архитектуры - переработать данный компонент
    public sealed class NetworkGamePlayer : NetworkBehaviour
    {
        [SerializeField] private PlayerMovement _player;
        [SyncVar] private string displayName = "Loading...";

        private MVNetworkManager room;

        private MVNetworkManager Room
        {
            get
            {
                if (room != null)
                {
                    return room;
                }

                return room = MVNetworkManager.singleton;
            }
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                _player.HasControl = true;
                _player.CharacterCamera.gameObject.SetActive(true);
            }

            if (isServer)
            {
                Room.HostMigrationState.PlayerMigrationStatus = HostMigrationStatus.I_AM_SERVER;
            }

            if (isOwned)
            {
                RemakeGame();
            }
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        [ClientRpc]
        public void StoreNewHostData(uint hostNetID)
        {
            //storing new hostData just incase current host leaves
            HostState hostState = new HostState();
            hostState.IP = IPUtils.GetLocalIPAddress();
            hostState.Port = Room.transport.ServerUri().Port;
            hostState.NetID = netId;
            hostState.SceneName = SceneManager.GetActiveScene().name;

            Room.HostMigrationState.NewHostCache = hostState;
            if (hostNetID == netId && isLocalPlayer)
            {
                Room.HostMigrationState.PlayerMigrationStatus = HostMigrationStatus.I_AM_NEW_HOST;
            }
        }

        private void RemakeGame()
        {
            if (Room.HostMigrationState.CurrentPlayerCache != null)
            {
                var previousPosition = Room.HostMigrationState.CurrentPlayerCache.PlayerPosition;
                var previousRotation = Room.HostMigrationState.CurrentPlayerCache.PlayerRotation;
                transform.position = new Vector3(previousPosition.x, previousPosition.y, previousPosition.z);
                transform.rotation =
                    new Quaternion(previousRotation.x, previousRotation.y, previousRotation.z, previousRotation.w);
            }
        }

        private void OnDestroy()
        {
            //when you are about to be destroyed
            //save your data to be reused on new host
            if (isLocalPlayer)
            {
                Room.HostMigrationState.CurrentPlayerCache = new PlayerCache()
                {
                    PlayerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    PlayerRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w)
                };
            }
        }
    }
}