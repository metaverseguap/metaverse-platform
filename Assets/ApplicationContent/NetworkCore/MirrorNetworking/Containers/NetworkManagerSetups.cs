using System.Collections.Generic;
using Mirror;
using NetworkCore.MirrorNetworking.Player.Base;
using NetworkCore.MirrorNetworking.Types.Devices;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Containers
{
    /// <summary>
    /// <para>Класс настроек <see cref="MVNetworkManager"/>.</para>
    /// </summary>
    public sealed class NetworkManagerSetups : MonoBehaviour
    {
        [Header("Build settings")] 
        [Tooltip("Устройство для которого производиться сборка проекта")] 
        [SerializeField] private Device _device;

        [Header("File server settings")] 
        [SerializeField] private string _serverUrl;

        [Header("Application Settings")] 
        [SerializeField] [Scene] private string _defaultScene;
        [SerializeField] [Scene] private string _menuScene;

        [SerializeField] private int _maxConnections;
        [SerializeField] private NetworkBasePlayer _networkPlayerPrefab;
        [Tooltip("Контроллеры игрока для различных устройств")]
        [SerializeField] private List<DevicePlayerPrefab> _devicePrefabs;

        /// <summary>
        /// Устройство для которого производиться сборка проекта.
        /// </summary>
        public Device Device => _device;

        /// <summary>
        /// Url файлового сервера.
        /// </summary>
        public string ServerUrl => _serverUrl;

        /// <summary>
        /// Сцена по умолчанию.
        /// </summary>
        public string DefaultScene => _defaultScene;
        
        /// <summary>
        /// Сцена меню.
        /// </summary>
        public string MenuScene => _menuScene;

        /// <summary>
        /// Максимальное количество подключений к серверу.
        /// </summary>
        public int MaxConnections => _maxConnections;

        /// <summary>
        /// Префаб сетевого игрока.
        /// </summary>
        public NetworkBasePlayer NetworkPlayerPrefab => _networkPlayerPrefab;

        /// <summary>
        /// Префабы игрока для различных устройств.
        /// </summary>
        public List<DevicePlayerPrefab> DevicePrefabs => _devicePrefabs;
    }
}