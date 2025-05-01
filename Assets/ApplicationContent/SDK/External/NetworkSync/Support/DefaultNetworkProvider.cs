using System;
using MV.SDK.Utils;

namespace LDR.SUAI_Metaverse.SDK.NetworkSync.Support
{
    /// <summary>
    /// <para>Класс заглушка, предоставляющий доступ к сетевым функциям.</para>
    /// </summary>
    public sealed class DefaultNetworkProvider : INetworkEnvironmentProvider
    {
        private const string DEFAULT_USERNAME = "Outsource user";

#pragma warning disable 0067 // CS0067: The event is never used
        public event Action OnConnectedToRoom;
#pragma warning restore 0067

        public string GetUsername()
        {
            return DEFAULT_USERNAME;
        }

        public bool IsConnectedToRoom()
        {
            return true;
        }

        public TimeSpan GetRoomLifetime()
        {
            return SceneUtils.GetSceneLifetime();
        }
    }
}