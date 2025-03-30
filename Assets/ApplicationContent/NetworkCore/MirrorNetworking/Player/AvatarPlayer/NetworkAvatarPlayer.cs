using AppAvatars;
using AppAvatars.Containers;
using Global.Logger;
using MainMenu.Containers;
using Mirror;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Player.Base;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Player.AvatarPlayer
{
    /// <summary>
    /// <para>Класс сетевого игрока с аватаром.</para>
    /// </summary>
    [RequireComponent(typeof(NetworkTransformReliable))]
    public class NetworkAvatarPlayer : NetworkBasePlayer
    {
        [SyncVar]
        private string avatarName = "";

        /// <summary>
        /// <para>Установить имя аватара игрока.</para>
        /// <remarks>данный метод выполняется на сервере.
        /// Это нужно, что бы локальная машина не затирала значения переменной других игроков своим локальным значением</remarks>
        /// </summary>
        /// <param name="avatarName">имя аватара игрока</param>
        [Server]
        public void SetAvatarName(string avatarName)
        {
            this.avatarName = avatarName;
        }

        /// <summary>
        /// <inheritdoc cref="NetworkBasePlayer.OnStart"/>
        /// </summary>
        protected override void OnStart()
        {
            // Сначала спавним аватар игрока
            CreateAvatar();

            // Затем активируем контроллер и проводим другие настройки сетевого игрока
            base.OnStart();
        }

        private void CreateAvatar()
        {
            if (string.IsNullOrEmpty(avatarName))
            {
                AppLogger.Error("Avatar Name is empty.");
                return;
            }

            NetworkDataStore networkStore = MVNetworkManager.singleton.NetworkStore;

            AvatarFile avatar = networkStore.Avatars.AvatarFiles[avatarName];

            AbstractPlayerAvatar playerAvatar = Instantiate(networkStore.Player.CurrentBuildPlayerAvatar, transform, false);

            AvatarPrefabInfo avatarPrefabInfo = new AvatarPrefabInfo();
            avatarPrefabInfo.Prefab = avatar.Model.GetComponent<Animator>();
            avatarPrefabInfo.ForGender = avatar.AvatarGender;
            playerAvatar.AvatarComponent.CreatePlayerFromAvatar(avatarPrefabInfo);

            NetworkTransformReliable transformSync = GetComponent<NetworkTransformReliable>();
            transformSync.target = playerAvatar.transform;
            
            // TODO: Синхронизация анимации
            
            NetworkPlayerDisplayName displayNameObject = Instantiate(networkStore.Player.DisplayName, playerAvatar.transform, false);
            displayNameObject.NetworkPlayer = this;

            PlayerController = playerAvatar.PlayerController;
        }
    }
}