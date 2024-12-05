using System.Collections.Generic;
using AppAvatars.Containers;
using Global.Logger;
using UnityEngine;

namespace AppAvatars.AvatarSetups
{
    /// <summary>
    /// <para>Класс создающий аватара игрока в сцене.</para>
    /// </summary>
    public class PlayerAvatar : MonoBehaviour
    {
        /// <summary>
        /// Список <see cref="AbstractAvatarSetup">настроек аватара</see>.
        /// </summary>
        [Tooltip("Список настроек аватара (наследников класса AbstractAvatarSetup)")] 
        [SerializeField] private List<AbstractAvatarSetup> _avatarSetups;
        [Tooltip("Если лень в ручную переносить объекты настроек - эта опция соберет все настройки на данном объекте в список")] 
        [SerializeField] private bool _autoCollectSetupsFromCurrentObject;

        /// <summary>
        /// Аниматор префаба аватара игрока.
        /// </summary>
        protected AvatarPrefabInfo spawnedAvatar;

        /// <summary>
        /// Аниматор префаба аватара игрока.
        /// </summary>
        public AvatarPrefabInfo SpawnedAvatar => spawnedAvatar;

        private bool isPlayerSpawned;

        private void Awake()
        {
            isPlayerSpawned = false;
        }

        /// <summary>
        /// <para>Создает аватар игрока из выбранного аниматора префаба игрока.</para>
        /// </summary>
        /// <param name="avatarPrefabInfo">выбранный <see cref="AvatarPrefabInfo">префаб игрока</see></param>
        public void CreatePlayerFromAvatar(AvatarPrefabInfo avatarPrefabInfo)
        {
            if (_autoCollectSetupsFromCurrentObject)
            {
                CollectSetupsFromCurrentObject();
            }
            
            if (DoesComponentContainErrors())
            {
                return;
            }

            if (!avatarPrefabInfo.Prefab.avatar.isHuman)
            {
                AppLogger.Error("The avatar's skeleton is not humanoid");
                return;
            }

            spawnedAvatar = new AvatarPrefabInfo();
            spawnedAvatar.Prefab = SpawnPlayer(avatarPrefabInfo.Prefab);
            spawnedAvatar.ForGender = avatarPrefabInfo.ForGender;
            
            AvatarSetup(ref spawnedAvatar);
            isPlayerSpawned = true;
        }

        private void CollectSetupsFromCurrentObject()
        {
            AbstractAvatarSetup[] collectedSetups = GetComponents<AbstractAvatarSetup>();
            foreach (var setup in collectedSetups)
            {
                if (!_avatarSetups.Contains(setup))
                {
                    _avatarSetups.Add(setup);
                }
            }
        }

        protected virtual bool DoesComponentContainErrors()
        {
            if (isPlayerSpawned)
            {
                return true;
            }

            foreach (var avatarSetup in _avatarSetups)
            {
                if (avatarSetup.ComponentContainsErrors())
                {
                    return true;
                }
            }

            return false;
        }

        private Animator SpawnPlayer(Animator avatarPrefab)
        {
            return Instantiate(avatarPrefab, transform, false);
        }

        private void AvatarSetup(ref AvatarPrefabInfo avatarPrefabInfo)
        {
            foreach (var avatarSetup in _avatarSetups)
            {
                avatarSetup.SetUp(ref avatarPrefabInfo);
            }
        }

        /// <summary>
        /// <para>Уничтожить аватар игрока.</para>
        /// </summary>
        public void DestroyPlayerAvatar()
        {
            if (spawnedAvatar?.Prefab != null)
            {
                Destroy(spawnedAvatar.Prefab.gameObject);
                spawnedAvatar = null;
            }
        }
    }
}