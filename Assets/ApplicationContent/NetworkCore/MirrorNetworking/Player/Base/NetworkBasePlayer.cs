using AppAvatars;
using AppAvatars.AvatarSetups;
using Mirror;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Player.Base
{
    /// <summary>
    /// <para>Основной класс сетевого игрока.</para>
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkBasePlayer : NetworkBehaviour
    {
        // Переменная синхронизирована с сервером
        [SyncVar] 
        private string displayName = "Loading...";
        [SyncVar] 
        private string avatarName = "";
        [SyncVar]
        private int connectionId = 0;
        
        private AbstractPlayer playerController;

        /// <summary>
        /// Отображаемое имя игрока.
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// Ссылка на контроллер игрока.
        /// </summary>
        public AbstractPlayer PlayerController
        {
            get => playerController;
            set
            {
                playerController = value;
                
                transform.SetParent(playerController.transform);
                MoveObjectToBoneSetup.MoveObjectToBone(transform, playerController.AvatarComponent.SpawnedAvatar.Prefab, HumanBodyBones.Head);
                transform.position += new Vector3(0, 0.5f, 0);
            }
        }

        /// <summary>
        /// <para>Установить отображаемое имя игрока.</para>
        ///
        /// <remarks>метод вызывается только на сервере</remarks>
        /// </summary>
        /// <param name="displayName">отображаемое имя игрока</param>
        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }
        
        /// <summary>
        /// <para>Установить имя аватара игрока.</para>
        ///
        /// <remarks>метод вызывается только на сервере</remarks>
        /// </summary>
        /// <param name="avatarName">имя аватара игрока</param>
        [Server]
        public void SetAvatarName(string avatarName)
        {
            this.avatarName = avatarName;
        }

        /// <summary>
        /// Имя аватара игрока.
        /// </summary>
        public string AvatarName => avatarName;

        /// <summary>
        /// <para>Установить id подключения.</para>
        ///
        /// <remarks>метод вызывается только на сервере</remarks>
        /// </summary>
        /// <param name="id">id подключения</param>
        [Server]
        public void SetConnectedId(int id)
        {
            this.connectionId = id;
        }

        /// <summary>
        /// ID подключения.
        /// </summary>
        public int ConnectionId => connectionId;
    }
}