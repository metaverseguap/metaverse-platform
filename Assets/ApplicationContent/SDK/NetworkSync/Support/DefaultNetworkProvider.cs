using System;
using LDR.SUAI_Metaverse.SDK.Utils;

namespace LDR.SUAI_Metaverse.SDK.NetworkSync.Support
{
    /// <summary>
    /// <para>Класс заглушка, предоставляющий доступ к сетевым функциям.</para>
    /// </summary>
    public sealed class DefaultNetworkProvider : INetworkEnvironmentProvider
    {
        private const string DEFAULT_USERNAME = "Outsource user";

#pragma warning disable 0067 // CS0067: The event is never used
        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.OnConnectedToRoom"/>
        /// </summary>
        public event Action OnConnectedToRoom;
#pragma warning restore 0067

        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.GetUsername"/>
        /// </summary>
        /// <returns><inheritdoc cref="INetworkEnvironmentProvider.GetUsername"/></returns>
        public string GetUsername()
        {
            return DEFAULT_USERNAME;
        }

        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.IsConnectedToRoom"/>
        /// </summary>
        /// <returns><inheritdoc cref="INetworkEnvironmentProvider.IsConnectedToRoom"/></returns>
        public bool IsConnectedToRoom()
        {
            return true;
        }

        /// <summary>
        /// <inheritdoc cref="INetworkEnvironmentProvider.GetRoomLifetime"/>
        /// </summary>
        /// <returns><inheritdoc cref="INetworkEnvironmentProvider.GetRoomLifetime"/></returns>
        public TimeSpan GetRoomLifetime()
        {
            return SceneUtils.GetSceneLifetime();
        }
    }
}