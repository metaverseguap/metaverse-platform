using System.Collections.Generic;
using NetworkCore.MirrorNetworking.Containers.ManagerSetups;
using NetworkCore.MirrorNetworking.Player.Base;
using NetworkCore.MirrorNetworking.Types.HostMigration;
using NetworkCore.ServerInteraction.API;

namespace NetworkCore.MirrorNetworking.Containers.Store
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
        /// <para>Хеш-коды зарегистрированных префабов.</para>
        ///
        /// Mirror не позволяет просто спавнить любые объекты в сцене.
        /// Объекты должны быть зарегистрированы в <c>NetworkManager.spawnPrefabs</c>.
        /// В данном сете хранятся хеш-коды зарегистрированных префабов,
        /// для быстрого поиска зарегистрированных префабов
        /// </summary>
        public ISet<int> RegisterPrefabsHash { get; } = new HashSet<int>();
        
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
            Player.DisplayName = setups.DisplayNamePrefab;
            Scenes.MenuSceneName = setups.MenuScene;
            Scenes.LoadingSceneName = setups.LoadingScene;
            foreach (var prefab in setups.DevicePrefabs)
            {
                if (prefab.ForDevice == setups.Device)
                {
                    Player.CurrentBuildPlayerAvatar = prefab.Prefab;
                    break;
                }
            }
        }
    }
}