namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище данных необходимых для миграции хоста.</para>
    /// </summary>
    public sealed class HostMigrationStore
    {
        /// <summary>
        /// Логин игрока, который станет хостом при отключении текущего хоста.
        /// </summary>
        public string BackupHostLogin { get; set; }

        /// <summary>
        /// Хранилище кешей.
        /// </summary>
        public CacheStore Caches { get; } = new CacheStore();

        /// <summary>
        /// Очистить данные миграции.
        /// </summary>
        public void Clear()
        {
            BackupHostLogin = null;
            Caches.Clear();
        }
    }
}