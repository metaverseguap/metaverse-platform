using System;
using LDR.SUAI_Metaverse.SDK.Core.Player;
using NetworkCore.MirrorNetworking.Player.Base;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище префабов, спавнемых во время игры.</para>
    /// </summary>
    public sealed class SpawnablePrefabsStore
    {
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="builder">builder</param>
        private SpawnablePrefabsStore(SpawnablePrefabsStoreBuilder builder)
        {
            NetworkPlayer = builder.NetworkPlayer;
            DisplayName = builder.DisplayName;
            CurrentBuildPlayer = builder.CurrentBuildPlayer;
        }

        /// <summary>
        /// Префаб сетевого игрока.
        /// </summary>
        public NetworkBasePlayer NetworkPlayer { get; }

        /// <summary>
        /// Префаб отображаемого имени сетевого игрока.
        /// </summary>
        public NetworkPlayerDisplayName DisplayName { get; }

        /// <summary>
        /// Префаб игрока для текущей сборки.
        /// </summary>
        public AbstractPlayer CurrentBuildPlayer { get; }

        /// <summary>
        /// Builder.
        /// </summary>
        public static SpawnablePrefabsStoreBuilder Builder => new SpawnablePrefabsStoreBuilder();

        /// <summary>
        /// Builder.
        /// </summary>
        public class SpawnablePrefabsStoreBuilder : IDisposable
        {
            /// <summary>
            /// Префаб сетевого игрока для текущей сборки.
            /// </summary>
            public NetworkBasePlayer NetworkPlayer { get; private set; }

            /// <summary>
            /// Префаб отображаемого имени сетевого игрока.
            /// </summary>
            public NetworkPlayerDisplayName DisplayName { get; private set; }

            /// <summary>
            /// Префаб игрока для текущей сборки.
            /// </summary>
            public AbstractPlayer CurrentBuildPlayer { get; private set; }

            /// <param name="player">префаб сетевого игрока для текущей сборки</param>
            /// <returns>self</returns>
            public SpawnablePrefabsStoreBuilder WithNetworkPlayer(NetworkBasePlayer player)
            {
                NetworkPlayer = player;
                return this;
            }

            /// <param name="displayName">префаб отображаемого имени сетевого игрока</param>
            /// <returns>self</returns>
            public SpawnablePrefabsStoreBuilder WithDisplayName(NetworkPlayerDisplayName displayName)
            {
                DisplayName = displayName;
                return this;
            }

            /// <param name="currentPlayer">рефаб игрока для текущей сборки</param>
            /// <returns>self</returns>
            public SpawnablePrefabsStoreBuilder WithCurrentBuildPlayer(AbstractPlayer currentPlayer)
            {
                CurrentBuildPlayer = currentPlayer;
                return this;
            }

            /// <summary>
            /// <para>Собрать объект класса.</para>
            /// </summary>
            /// <returns>объект класса</returns>
            public SpawnablePrefabsStore Build()
            {
                return new SpawnablePrefabsStore(this);
            }

            /// <summary>
            /// <para>Освобождение ресурсов GC.</para> 
            /// </summary>
            public void Dispose()
            {
                NetworkPlayer = null;
                DisplayName = null;
                CurrentBuildPlayer = null;
            }
        }
    }
}