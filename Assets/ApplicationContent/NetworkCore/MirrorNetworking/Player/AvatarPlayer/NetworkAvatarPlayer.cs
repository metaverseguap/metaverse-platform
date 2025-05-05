using System.Collections.Generic;
using AppAvatars.AvatarSetups;
using AppAvatars.Containers;
using Cinemachine;
using Global.Logger;
using LDR.SUAI_Metaverse.SDK.Utils;
using MainMenu.Containers;
using Mirror;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Containers.Store.Cache;
using NetworkCore.MirrorNetworking.Containers.Synchronization;
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

        [SyncVar]
        private PlayerCache playerCache;

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
        /// <para>Установить кеш игрока.</para>
        /// <remarks>данный метод выполняется на сервере.
        /// Это нужно, что бы локальная машина не затирала значения переменной других игроков своим локальным значением</remarks>
        /// </summary>
        /// <param name="playerCache">кеш игрока</param>
        [Server]
        public void SetCache(PlayerCache playerCache)
        {
            this.playerCache = playerCache;
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
            ConfigurationStore configuration = networkStore.Configuration;
            AvatarStore avatarStore = networkStore.FileStore.Avatars;
            AvatarFile avatar = avatarStore.AvatarFiles[avatarName];
            Animator prefab = avatar.Model.GetComponent<Animator>();

            AbstractPlayer player = Instantiate(configuration.SpawnablePrefabs.CurrentBuildPlayer, transform, false);
            PlayerAvatar avatarComponent = player.AvatarComponent;

            AddAnimatorControllerToPrefab(avatar, configuration.AvatarConfiguration.AnimatorControllers, ref prefab);

            avatarComponent.CreatePlayerFromAvatar(prefab);

            SetPlayerTransformSync(ref player);

            SetAvatarVisibility(ref avatarComponent);

            Animator spawnedAvatar = avatarComponent.SpawnedAvatar;

            SetAvatarAnimationSync(ref spawnedAvatar);

            SpawnNetworkDisplayName(networkStore, player);

            PlayerController = player.PlayerController;
        }

        private static void AddAnimatorControllerToPrefab(AvatarFile avatar, List<AnimatorControllerInfo> animatorControllers, ref Animator prefab)
        {
            foreach (var controller in animatorControllers)
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

            if (playerCache != null && playerCache.IsInitialized)
            {
                SetTransformFromCache(ref player);
            }
        }

        private void SetTransformFromCache(ref AbstractPlayer player)
        {
            player.transform.position = playerCache.PlayerPosition;
            player.transform.rotation = playerCache.PlayerRotation;

            SetCameraRotationsFromCache(ref player);
        }

        private void SetCameraRotationsFromCache(ref AbstractPlayer player)
        {
            IDictionary<string, Quaternion> cameraRotations = new Dictionary<string, Quaternion>();
            NamedTransform[] playerCameraRotations = playerCache.PlayerCameraRotations;
            foreach (var cameraRotation in playerCameraRotations)
            {
                cameraRotations.Add(cameraRotation.Name, cameraRotation.Rotation);
            }

            CinemachineVirtualCamera[] playerCameras = player.GetComponentsInChildren<CinemachineVirtualCamera>(true);
            foreach (var playerCamera in playerCameras)
            {
                string objAbsolutePath = SceneUtils.GetObjectPathFromRoot(playerCamera.transform, transform);
                if (cameraRotations.TryGetValue(objAbsolutePath, out Quaternion cameraRotation))
                {
                    playerCamera.transform.rotation = cameraRotation;
                }
            }
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
            AnimatorParameterListener parameterListener = spawnedAvatar.gameObject.AddComponent<AnimatorParameterListener>();
            MVNetworkAnimator animatorSync = GetComponent<MVNetworkAnimator>();
            animatorSync.ParameterListener = parameterListener;
            animatorSync.ClientAuthority = true;
        }

        private void SpawnNetworkDisplayName(NetworkDataStore networkStore, AbstractPlayer player)
        {
            NetworkPlayerDisplayName displayNameObject = Instantiate(networkStore.Configuration.SpawnablePrefabs.DisplayName, player.transform, false);
            displayNameObject.NetworkPlayer = this;
        }
    }
}