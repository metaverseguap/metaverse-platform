namespace NetworkCore.ServerInteraction.Type.Host.Request
{
    /// <summary>
    /// <para>Объект запроса создания хоста.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class CreateHostRequest
    {
        /// <summary>
        /// Uri идентификатор хоста, для подключения к нему через Mirror
        /// </summary>
        public string uri { get; set; }

        /// <summary>
        /// Название файла сцены, хостом которой является создаваемый хост
        /// </summary>
        public string sceneName { get; set; }
    }
}