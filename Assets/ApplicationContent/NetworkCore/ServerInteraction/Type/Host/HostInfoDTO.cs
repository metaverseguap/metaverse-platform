namespace NetworkCore.ServerInteraction.Type.Host
{
    /// <summary>
    /// <para>Объект передачи данных информации о хосте.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class HostInfoDTO
    {
        /// <summary>
        /// Uri идентификатор хоста, для подключения к нему через Mirror.
        /// </summary>
        public string uri { get; set; }

        /// <summary>
        /// Название файла сцены, хостом которой является данный хост.
        /// </summary>
        public string sceneName { get; set; }

        /// <summary>
        /// Логин пользователя являющегося хостом.
        /// </summary>
        public string login { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя являющегося хостом.
        /// </summary>
        public string name { get; set; }
    }
}