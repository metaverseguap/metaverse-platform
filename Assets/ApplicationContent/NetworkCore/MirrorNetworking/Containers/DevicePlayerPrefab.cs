using System;
using AppAvatars;
using NetworkCore.MirrorNetworking.Types.Devices;

namespace NetworkCore.MirrorNetworking.Containers
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