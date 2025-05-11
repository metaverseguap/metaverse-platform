using Mirror;

namespace NetworkCore.MirrorNetworking.ClientMessages
{
    /// <summary>
    /// <para>Сообщение, передающее новому клиенту данные о состоянии комнаты.</para>
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
        /// Массив NetId списка игроков.
        /// </summary>
        public readonly uint[] GamePlayersNetIds;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="initialized">инициализировано ли состояние комнаты</param>
        /// <param name="roomStartTimeTicks">количество тиков прошедших с создания комнаты</param>
        /// <param name="gamePlayersNetIds">массив NetId списка игроков</param>
        public RoomStateMessage(bool initialized, long roomStartTimeTicks, uint[] gamePlayersNetIds)
        {
            RoomStartTimeTicks = roomStartTimeTicks;
            GamePlayersNetIds = gamePlayersNetIds;
            Initialized = initialized;
        }
    }
}