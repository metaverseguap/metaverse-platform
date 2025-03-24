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
        /// IP хоста, для подключения к нему через Mirror.
        /// </summary>
        public string hostIP { get; set; }
        
        /// <summary>
        /// Порт хоста.
        /// </summary>
        public int port { get; set; }

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