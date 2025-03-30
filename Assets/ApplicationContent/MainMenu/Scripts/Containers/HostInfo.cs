namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер информации о хосте.</para>
    /// </summary>
    public sealed class HostInfo
    {
        /// <summary>
        /// IP хоста, для подключения к нему через Mirror.
        /// </summary>
        public string HostIP { get; set; }
        
        /// <summary>
        /// Порт хоста.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Название файла сцены, хостом которой является данный хост.
        /// </summary>
        public string SceneName { get; set; }

        /// <summary>
        /// Логин пользователя являющегося хостом.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя являющегося хостом.
        /// </summary>
        public string DisplayName { get; set; }
    }
}