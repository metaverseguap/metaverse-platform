using System;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище игровых настроек, не изменяющихся в процессе игры.</para>
    /// </summary>
    public sealed class ConfigurationStore
    {
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="builder">builder</param>
        private ConfigurationStore(ConfigurationStoreBuilder builder)
        {
            SceneConfiguration = builder.SceneConfiguration;
            SpawnablePrefabs = builder.SpawnablePrefabs;
            AvatarConfiguration = builder.AvatarConfiguration;
        }

        /// <summary>
        /// Хранилище настройки сцен.
        /// </summary>
        public SceneConfigurationStore SceneConfiguration { get; }

        /// <summary>
        /// Хранилище префабов, спавнемых во время игры.
        /// </summary>
        public SpawnablePrefabsStore SpawnablePrefabs { get; }

        /// <summary>
        /// Хранилище настройки аватаров.
        /// </summary>
        public AvatarConfigurationStore AvatarConfiguration { get; }

        /// <summary>
        /// Builder.
        /// </summary>
        public static ConfigurationStoreBuilder Builder => new ConfigurationStoreBuilder();

        /// <summary>
        /// Builder.
        /// </summary>
        public sealed class ConfigurationStoreBuilder : IDisposable
        {
            /// <summary>
            /// Хранилище настройки сцен.
            /// </summary>
            public SceneConfigurationStore SceneConfiguration { get; private set; }

            /// <summary>
            /// Хранилище префабов, спавнемых во время игры.
            /// </summary>
            public SpawnablePrefabsStore SpawnablePrefabs { get; private set; }

            /// <summary>
            /// Хранилище настройки аватаров.
            /// </summary>
            public AvatarConfigurationStore AvatarConfiguration { get; private set; }

            /// <param name="sceneConfiguration">хранилище настройки сцен</param>
            /// <returns>self</returns>
            public ConfigurationStoreBuilder WithSceneConfiguration(SceneConfigurationStore sceneConfiguration)
            {
                SceneConfiguration = sceneConfiguration;
                return this;
            }

            /// <param name="spawnablePrefabs">хранилище префабов, спавнемых во время игры</param>
            /// <returns>self</returns>
            public ConfigurationStoreBuilder WithSpawnablePrefabs(SpawnablePrefabsStore spawnablePrefabs)
            {
                SpawnablePrefabs = spawnablePrefabs;
                return this;
            }

            /// <param name="avatarConfiguration">хранилище настройки аватаров</param>
            /// <returns>self</returns>
            public ConfigurationStoreBuilder WithAvatarConfiguration(AvatarConfigurationStore avatarConfiguration)
            {
                AvatarConfiguration = avatarConfiguration;
                return this;
            }

            /// <summary>
            /// <para>Собрать объект класса.</para>
            /// </summary>
            /// <returns>объект класса</returns>
            public ConfigurationStore Build()
            {
                return new ConfigurationStore(this);
            }

            /// <summary>
            /// <para>Освобождение ресурсов GC.</para> 
            /// </summary>
            public void Dispose()
            {
                SpawnablePrefabs = null;
                SceneConfiguration = null;
                AvatarConfiguration = null;
            }
        }
    }
}