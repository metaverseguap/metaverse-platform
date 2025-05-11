using NetworkCore.MirrorNetworking.Containers.Synchronization;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Containers.Store.Cache
{
    /// <summary>
    /// <para>Кеш игрока.</para>
    ///
    /// Данный класс необходим для временного хранения состояния игрока при миграции хоста
    /// </summary>
    [System.Serializable]
    public sealed class PlayerCache
    {
        /// <summary>
        /// Инициализирован ли кеш игрока.
        /// </summary>
        public bool IsInitialized = false;

        /// <summary>
        /// Положение игрока.
        /// </summary>
        public Vector3 PlayerPosition;

        /// <summary>
        /// Поворот игрока.
        /// </summary>
        public Quaternion PlayerRotation;

        /// <summary>
        /// Повороты камер игрока.
        /// </summary>
        public NamedTransform[] PlayerCameraRotations;

        /// <summary>
        /// <para>Очистить кеш.</para>
        /// </summary>
        public void Clear()
        {
            IsInitialized = false;
            PlayerPosition = Vector3.zero;
            PlayerRotation = Quaternion.identity;
            PlayerCameraRotations = null;
        }
    }
}