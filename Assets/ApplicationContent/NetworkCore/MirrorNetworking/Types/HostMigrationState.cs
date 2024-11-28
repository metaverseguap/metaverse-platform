namespace NetworkCore.MirrorNetworking.Types
{
    /// <summary>
    /// <para>Данные необходимые для переподключения игрока, при миграции хоста.</para>
    /// </summary>
    public class HostMigrationState
    {
        /// <summary>
        /// <see cref="PlayerCache">Кеш игрока</see>.
        /// </summary>
        public PlayerCache CurrentPlayerCache { get; set; }
        /// <summary>
        /// <see cref="HostState">Кеш хоста</see>, к которому необходимо подключится во время миграции.
        /// </summary>
        public HostState NewHostCache { get; set; } = new HostState();
        /// <summary>
        /// <see cref="HostMigrationStatus">Статус</see> текущего игрока во время миграции хоста.
        /// </summary>
        public HostMigrationStatus PlayerMigrationStatus { get; set; } = HostMigrationStatus.NONE;
    }
}