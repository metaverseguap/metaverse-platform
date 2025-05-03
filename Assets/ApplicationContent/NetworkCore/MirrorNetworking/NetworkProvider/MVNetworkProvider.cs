using System;
using LDR.SUAI_Metaverse.SDK.NetworkSync.Support;
using LDR.SUAI_Metaverse.SDK.Utils;

namespace NetworkCore.MirrorNetworking.NetworkProvider
{
    /// <summary>
    /// <para>Класс, предоставляющий доступ к сетевым функциям для внешних систем.</para>
    /// </summary>
    public sealed class MVNetworkProvider : INetworkEnvironmentProvider
    {
        private const string DEFAULT_USERNAME = "Offline player";

        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.OnConnectedToRoom"/>
        /// </summary>
        public event Action OnConnectedToRoom;

        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.GetUsername"/>
        /// </summary>
        /// <returns><inheritdoc cref="INetworkEnvironmentProvider.GetUsername"/></returns>
        public string GetUsername()
        {
            if (MVNetworkManager.IsOnline())
            {
                return MVNetworkManager.singleton.NetworkStore.FileServer.User.GetMyUser().Nickname;
            }

            return DEFAULT_USERNAME;
        }

        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.IsConnectedToRoom"/>
        /// </summary>
        /// <returns><inheritdoc cref="INetworkEnvironmentProvider.IsConnectedToRoom"/></returns>
        public bool IsConnectedToRoom()
        {
            if (MVNetworkManager.IsOnline())
            {
                return MVNetworkManager.singleton.NetworkStore.Room.Initialized;
            }

            return true;
        }

        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.GetRoomLifetime"/>
        /// </summary>
        /// <returns><inheritdoc cref="INetworkEnvironmentProvider.GetRoomLifetime"/></returns>
        public TimeSpan GetRoomLifetime()
        {
            if (MVNetworkManager.IsOnline()
                && MVNetworkManager.singleton.NetworkStore.Room.Initialized)
            {
                DateTime roomCreationTime = MVNetworkManager.singleton.NetworkStore.Room.RoomStartTime;
                return DateTime.UtcNow - roomCreationTime;
            }

            return SceneUtils.GetSceneLifetime();
        }

        /// <summary>
        /// <para>Сообщает сетевому окружению об успешном подключении к комнате.</para>
        /// </summary>
        public void NotifyRoomConnection()
        {
            OnConnectedToRoom?.Invoke();
        }
    }
}