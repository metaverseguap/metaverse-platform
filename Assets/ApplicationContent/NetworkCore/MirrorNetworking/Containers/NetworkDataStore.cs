using System.Collections.Generic;
using NetworkCore.MirrorNetworking.Player.Base;
using NetworkCore.MirrorNetworking.Types.HostMigration;
using NetworkCore.ServerInteraction.API;
using UserSystem.RoleSystem.Types;

namespace NetworkCore.MirrorNetworking.Containers
{
    /// <summary>
    /// <para>Хранилище данных, используемых при сетевом взаимодействии.</para>
    /// </summary>
    public sealed class NetworkDataStore
    {

        /// <summary>
        /// Хранилище данных об игроке.
        /// </summary>
        public PlayerStore Player { get; } = new PlayerStore();
        
        /// <summary>
        /// <para>Список игроков в комнате.</para>
        ///
        /// Ключем является connection id игрока
        /// </summary>
        public IDictionary<int, NetworkBasePlayer> GamePlayers { get; } = new Dictionary<int, NetworkBasePlayer>();
        
        /// <summary>
        /// Контейнер состояния игрока при миграции хоста.
        /// </summary>
        public HostMigrationState MigrationState { get; } = new HostMigrationState();

        /// <summary>
        /// Доступ к файловому серверу.
        /// </summary>
        public APIContainer FileServer { get; private set; }

        /// <summary>
        /// Хранилище аватаров.
        /// </summary>
        public AvatarStore Avatars { get; } = new AvatarStore();
        
        /// <summary>
        /// Хранилище сцен.
        /// </summary>
        public SceneStore Scenes { get; } = new SceneStore();

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="setups"><see cref="NetworkManagerSetups"/></param>
        public NetworkDataStore(NetworkManagerSetups setups)
        {
            FileServer = new APIContainer(setups.ServerUrl);
            Player.NetworkPlayer = setups.NetworkPlayerPrefab;
            Scenes.MenuSceneName = setups.MenuScene;
            foreach (var prefab in setups.DevicePrefabs)
            {
                if (prefab.ForDevice == setups.Device)
                {
                    Player.CurrentBuildPlayer = prefab.Prefab;
                    break;
                }
            }
        }
    }
}