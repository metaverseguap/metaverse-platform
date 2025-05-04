using System.Collections.Generic;
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
        protected Animator spawnedAvatar;

        /// <summary>
        /// Аниматор префаба аватара игрока.
        /// </summary>
        public Animator SpawnedAvatar => spawnedAvatar;

        private bool isPlayerSpawned;

        private void Awake()
        {
            isPlayerSpawned = false;
        }

        /// <summary>
        /// <para>Создает аватар игрока из выбранного аниматора префаба игрока.</para>
        /// </summary>
        /// <param name="avatarPrefab">выбранный префаб игрока, имеющий аниматор</param>
        public void CreatePlayerFromAvatar(Animator avatarPrefab)
        {
            if (_autoCollectSetupsFromCurrentObject)
            {
                CollectSetupsFromCurrentObject();
            }
            
            if (DoesComponentContainErrors())
            {
                return;
            }

            spawnedAvatar = SpawnPlayer(avatarPrefab);
            
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

        private void AvatarSetup(ref Animator avatarPrefab)
        {
            foreach (var avatarSetup in _avatarSetups)
            {
                avatarSetup.SetUp(ref avatarPrefab);
            }
        }

        /// <summary>
        /// <para>Установить видимость аватара для главной камеры.</para>
        /// </summary>
        /// <param name="isVisible">true, если главная камера должна видеть данный аватар</param>
        public void SetAvatarVisibility(bool isVisible)
        {
            if (!isPlayerSpawned)
            {
                AppLogger.Error($"Player avatar is not spawned yet.");
                return;
            }
            
            int layer = LayerMask.NameToLayer("Avatar");
            if (!isVisible)
            {
                layer = LayerMask.NameToLayer("Not rendered");
            }
            
            SetLayerRecursively(spawnedAvatar.gameObject, layer);
        }
        
        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
        
        /// <summary>
        /// <para>Уничтожить аватар игрока.</para>
        /// </summary>
        public void DestroyPlayerAvatar()
        {
            if (spawnedAvatar != null)
            {
                Destroy(spawnedAvatar.gameObject);
                spawnedAvatar = null;
            }
        }
    }
}