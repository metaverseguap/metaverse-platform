using System;
using Mirror;

namespace NetworkCore.MirrorNetworking.ClientMessages
{
    /// <summary>
    /// <para>Сообщение, передающее новому клиенту данные о миграции хоста.</para>
    /// </summary>
    public struct RoomStateMessage : NetworkMessage
    {
        /// <summary>
        /// Инициализировано ли состояние комнаты.
        /// </summary>
        public readonly bool Initialized;

        /// <summary>
        /// Количество тиков прошедших с создания комнаты.
        /// </summary>
        public readonly long RoomStartTimeTicks;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="initialized">инициализировано ли состояние комнаты</param>
        /// <param name="roomStartTimeTicks">количество тиков прошедших с создания комнаты</param>
        public RoomStateMessage(bool initialized, long roomStartTimeTicks)
        {
            RoomStartTimeTicks = roomStartTimeTicks;
            Initialized = initialized;
        }
    }
}