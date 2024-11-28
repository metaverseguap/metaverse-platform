using UnityEngine;

namespace NetworkCore.MirrorNetworking.Types
{
    /// <summary>
    /// <para>Кеш игрока.</para>
    ///
    /// Данный класс необходим для временного хранения состояния игрока при миграции хоста
    /// </summary>
    public class PlayerCache
    {
        /// <summary>
        /// Положение игрока.
        /// </summary>
        public Vector3 PlayerPosition { get; set; } = Vector3.zero;
        /// <summary>
        /// Поворот игрока.
        /// </summary>
        public Quaternion PlayerRotation { get; set; } = Quaternion.identity;
    }
}