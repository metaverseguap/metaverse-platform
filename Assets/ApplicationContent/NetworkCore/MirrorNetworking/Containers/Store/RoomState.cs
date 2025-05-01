using System;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Данные состояния текущей комнаты.</para>
    /// </summary>
    public sealed class RoomState
    {
        /// <summary>
        /// Инициализировано ли состояние комнаты.
        /// </summary>
        public bool Initialized { get; set; } = false;

        /// <summary>
        /// Время прошедшее с создания комнаты.
        /// </summary>
        public DateTime RoomStartTime { get; set; }
    }
}