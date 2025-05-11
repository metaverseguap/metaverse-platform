using System;
using System.Collections.Generic;
using NetworkCore.MirrorNetworking.Player.Base;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Данные состояния текущей комнаты.</para>
    /// </summary>
    public sealed class RoomStateStore
    {
        /// <summary>
        /// Инициализировано ли состояние комнаты.
        /// </summary>
        public bool Initialized { get; set; } = false;

        /// <summary>
        /// Время, прошедшее с создания комнаты.
        /// </summary>
        public DateTime RoomStartTime { get; set; }

        /// <summary>
        /// <para>Список игроков в комнате.</para>
        ///
        /// Ключом является netId игрока
        /// </summary>
        public IDictionary<uint, NetworkBasePlayer> GamePlayers { get; } = new Dictionary<uint, NetworkBasePlayer>();

        /// <summary>
        /// Очистить хранилище.
        /// </summary>
        public void Clear()
        {
            Initialized = false;
            RoomStartTime = DateTime.UtcNow;
            GamePlayers.Clear();
        }
    }
}