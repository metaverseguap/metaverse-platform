using NetworkCore.MirrorNetworking.Containers.Store.Cache;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище кешей.</para>
    /// </summary>
    public sealed class CacheStore
    {
        /// <summary>
        /// <see cref="PlayerCache">Кеш игрока</see>.
        /// </summary>
        public PlayerCache CurrentPlayerCache { get; } = new PlayerCache();

        /// <summary>
        /// <see cref="SceneCache">Кеш сцены</see>.
        /// </summary>
        public SceneCache CurrentSceneCache { get; } = new SceneCache();

        /// <summary>
        /// <para>Очистить кеш.</para>
        /// </summary>
        public void Clear()
        {
            CurrentPlayerCache.Clear();
            CurrentSceneCache.Clear();
        }
    }
}