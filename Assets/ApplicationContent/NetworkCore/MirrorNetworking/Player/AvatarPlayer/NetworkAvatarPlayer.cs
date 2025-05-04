using AppAvatars.AvatarSetups;
using Global.Logger;
using MainMenu.Containers;
using Mirror;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Player.Base;
using NetworkCore.MirrorNetworking.Synchronization.Animations;
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
            Animator prefab = avatar.Model.GetComponent<Animator>();

            AbstractPlayer player = Instantiate(networkStore.Player.CurrentBuildPlayer, transform, false);
            PlayerAvatar avatarComponent = player.AvatarComponent;

            AddAnimatorControllerToPrefab(avatar, avatarStore, ref prefab);

            avatarComponent.CreatePlayerFromAvatar(prefab);

            SetPlayerTransformSync(ref player);

            SetAvatarVisibility(ref avatarComponent);

            Animator spawnedAvatar = avatarComponent.SpawnedAvatar;

            SetAvatarAnimationSync(ref spawnedAvatar);

            SpawnNetworkDisplayName(networkStore, player);

            PlayerController = player.PlayerController;
        }

        private static void AddAnimatorControllerToPrefab(AvatarFile avatar, AvatarStore avatarStore, ref Animator prefab)
        {
            foreach (var controller in avatarStore.AnimatorControllers)
            {
                if (controller.AnimationControllerType == avatar.AvatarAnimationControllerType)
                {
                    prefab.runtimeAnimatorController = controller.Controller;
                    break;
                }
            }
        }

        private void SetPlayerTransformSync(ref AbstractPlayer player)
        {
            NetworkTransformReliable transformSync = GetComponent<NetworkTransformReliable>();
            transformSync.target = player.transform;
        }

        private void SetAvatarVisibility(ref PlayerAvatar avatarComponent)
        {
            bool isCurrentPlayerAvatar = isClient && isLocalPlayer;
            if (isCurrentPlayerAvatar)
            {
                avatarComponent.SetAvatarVisibility(false);
            }
            else
            {
                avatarComponent.SetAvatarVisibility(true);
            }
        }

        private void SetAvatarAnimationSync(ref Animator spawnedAvatar)
        {
            AnimatorParameterListener parameterListener =gameObject.AddComponent<AnimatorParameterListener>();
            MVNetworkAnimator animatorSync = GetComponent<MVNetworkAnimator>();
            animatorSync.ParameterListener = parameterListener;
            animatorSync.ClientAuthority = true;
        }

        private void SpawnNetworkDisplayName(NetworkDataStore networkStore, AbstractPlayer player)
        {
            NetworkPlayerDisplayName displayNameObject = Instantiate(networkStore.Player.DisplayName, player.transform, false);
            displayNameObject.NetworkPlayer = this;
        }
    }
}