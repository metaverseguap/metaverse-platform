using System;
using NetworkCore.MirrorNetworking.Types.Devices;
using Player.EmbeddedPlayers;

namespace NetworkCore.MirrorNetworking.Containers.ManagerSetups
{
    /// <summary>
    /// <para>Контейнер хранящий префаб игрока и устройство для которого этот префаб применяется.</para>
    /// </summary>
    [Serializable]
    public sealed class DevicePlayerPrefab
    {
        public Device ForDevice;
        public AbstractPlayer Prefab;
    }
}