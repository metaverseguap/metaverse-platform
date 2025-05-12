using NetworkCore.MirrorNetworking.Containers.ManagerSetups;
using NetworkCore.MirrorNetworking.NetworkProvider;
using NetworkCore.ServerInteraction.API;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище данных, используемых при сетевом взаимодействии.</para>
    /// </summary>
    public sealed class NetworkDataStore
    {
        /// <summary>
        /// Провайдер сетевых функций <see cref="MVNetworkManager"/> для внешних систем.
        /// </summary>
        public MVNetworkProvider NetworkProvider { get; } = new MVNetworkProvider();

        /// <summary>
        /// Доступ к файловому серверу.
        /// </summary>
        public APIContainer FileServer { get; }

        /// <summary>
        /// Хранилище игровых настроек, не изменяющихся в процессе игры.
        /// </summary>
        public ConfigurationStore Configuration { get; }

        /// <summary>
        /// Хранилище файлов файлового сервера.
        /// </summary>
        public FileStore FileStore { get; } = new FileStore();

        /// <summary>
        /// Хранилище данных о регистрируемых в Mirror объектах.
        /// </summary>
        public MirrorRegisteredObjectsStore MirrorRegisteredObjects { get; } = new MirrorRegisteredObjectsStore();

        /// <summary>
        /// Хранилище данных об игроке.
        /// </summary>
        public PlayerStore MyPlayerInfo { get; } = new PlayerStore();

        /// <summary>
        /// Хранилище данных, необходимых для подключения игрока к сетевой сцене.
        /// </summary>
        public ConnectionStore Connection { get; } = new ConnectionStore();

        /// <summary>
        /// Хранилище состояния текущей комнаты.
        /// </summary>
        public RoomStateStore RoomState { get; } = new RoomStateStore();

        /// <summary>
        /// Хранилище данных, необходимых для миграции хоста.
        /// </summary>
        public HostMigrationStore HostMigration { get; } = new HostMigrationStore();

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="setups"><see cref="NetworkManagerSetups"/></param>
        public NetworkDataStore(NetworkManagerSetups setups)
        {
            FileServer = new APIContainer(setups);

            Configuration =
                ConfigurationStore.Builder
                    .WithSceneConfiguration(
                        SceneConfigurationStore.Builder
                            .WithMenuSceneName(setups.MenuScene)
                            .WithLoadingSceneName(setups.LoadingScene)
                            .Build()
                    )
                    .WithSpawnablePrefabs(
                        SpawnablePrefabsStore.Builder
                            .WithNetworkPlayer(setups.NetworkPlayerPrefab)
                            .WithDisplayName(setups.DisplayNamePrefab)
                            .WithCurrentBuildPlayer(setups.CurrentBuildPlayer)
                            .Build()
                    )
                    .WithAvatarConfiguration(
                        AvatarConfigurationStore.Builder
                            .WithAnimatorControllers(setups.AvatarAnimationControllers)
                            .Build()
                    )
                    .Build();
        }
    }
}