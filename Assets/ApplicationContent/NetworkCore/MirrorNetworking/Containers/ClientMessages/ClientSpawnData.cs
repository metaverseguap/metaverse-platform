using NetworkCore.MirrorNetworking.Containers.Synchronization;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Containers.ClientMessages
{
    /// <summary>
    /// <para>Контейнер для данных клиента необходимых для спавна клиента на сервере.</para>
    /// </summary>
    public sealed class ClientSpawnData
    {
        /// <summary>
        /// Название аватара игрока.
        /// </summary>
        public string Avatar;

        /// <summary>
        /// Логин игрока.
        /// </summary>
        public string Login;

        /// <summary>
        /// Отображаемое имя игрока.
        /// </summary>
        public string Nickname;

        /// <summary>
        /// Закешировано ли положение игрока.
        /// </summary>
        public bool IsTransformCached = false;

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
    }
}