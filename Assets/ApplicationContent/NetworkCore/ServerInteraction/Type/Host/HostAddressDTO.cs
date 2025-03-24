namespace NetworkCore.ServerInteraction.Type.Host
{
    /// <summary>
    /// <para>Объект передачи данных информации об адресе хоста.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class HostAddressDTO
    {
        /// <summary>
        /// IP хоста
        /// </summary>
        public string hostIP { get; set; }

        /// <summary>
        /// Порт хоста
        /// </summary>
        public int port { get; set; }
    }
}