using Global.Logger;
using MainMenu.Containers;
using Mirror;
using NetworkCore.MirrorNetworking.Animations;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Player.Base;
using Player.EmbeddedPlayers;
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
            AvatarStore avatarStore = networkStore.Avatars;
            AvatarFile avatar = avatarStore.AvatarFiles[avatarName];

            AbstractPlayer player = Instantiate(networkStore.Player.CurrentBuildPlayer, transform, false);

            Animator prefab = avatar.Model.GetComponent<Animator>();
            foreach (var controller in avatarStore.AnimatorControllers)
            {
                if (controller.AnimationControllerType == avatar.AvatarAnimationControllerType)
                {
                    prefab.runtimeAnimatorController = controller.Controller;
                    break;
                }
            }
     
            player.AvatarComponent.CreatePlayerFromAvatar(prefab);

            NetworkTransformReliable transformSync = GetComponent<NetworkTransformReliable>();
            transformSync.target = player.transform;
            
            Animator spawnedAvatar = player.AvatarComponent.SpawnedAvatar;
            AnimatorParameterListener parameterListener = spawnedAvatar.gameObject.AddComponent<AnimatorParameterListener>();
            MVNetworkAnimator animatorSync = GetComponent<MVNetworkAnimator>();
            animatorSync.ParameterListener = parameterListener;

            NetworkPlayerDisplayName displayNameObject = Instantiate(networkStore.Player.DisplayName, player.transform, false);
            displayNameObject.NetworkPlayer = this;

            PlayerController = player.PlayerController;
        }
    }
}