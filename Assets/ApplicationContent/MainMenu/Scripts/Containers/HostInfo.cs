namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер информации о хосте.</para>
    /// </summary>
    public sealed class HostInfo
    {
        /// <summary>
        /// Uri идентификатор хоста, для подключения к нему через Mirror.
        /// </summary>
        public string Uri { get; set; }

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