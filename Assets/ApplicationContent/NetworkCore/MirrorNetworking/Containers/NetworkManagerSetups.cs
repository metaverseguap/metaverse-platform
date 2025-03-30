using System.Collections.Generic;
using Mirror;
using NetworkCore.MirrorNetworking.Player.AvatarPlayer;
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
        [Tooltip("Сцена, в которой игрок появляется после выбора аватара - она же Offline Scene")] 
        [SerializeField] [Scene] private string _defaultScene;
        [Tooltip("Сцена, в которой игрок появляется после выхода из метавселенной")] 
        [SerializeField] [Scene] private string _menuScene;
        [Tooltip("Промежуточная сцена, которая подгружает необходимые ресурсы с сервера. В данную сцену переходит игрок при смене комнаты")] 
        [SerializeField] [Scene] private string _loadingScene;

        [SerializeField] private int _maxConnections;
        [SerializeField] private NetworkAvatarPlayer _networkPlayerPrefab;
        [SerializeField] private NetworkPlayerDisplayName _displayNamePrefab;
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
        /// Сцена загрузки между комнатами.
        /// </summary>
        public string LoadingScene => _loadingScene;

        /// <summary>
        /// Максимальное количество подключений к серверу.
        /// </summary>
        public int MaxConnections => _maxConnections;

        /// <summary>
        /// Префаб сетевого игрока.
        /// </summary>
        public NetworkBasePlayer NetworkPlayerPrefab => _networkPlayerPrefab;
        
        /// <summary>
        /// Префаб отображаемого имени игрока.
        /// </summary>
        public NetworkPlayerDisplayName DisplayNamePrefab => _displayNamePrefab;

        /// <summary>
        /// Префабы игрока для различных устройств.
        /// </summary>
        public List<DevicePlayerPrefab> DevicePrefabs => _devicePrefabs;
    }
}