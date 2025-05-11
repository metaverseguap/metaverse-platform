using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Host.Response
{
    /// <summary>
    /// <para>Объект результата запроса получения информации о хосте.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class HostResponse : ResponseDetails
    {
        /// <summary>
        /// Информация о хосте.
        /// </summary>
        public HostInfoDTO host { get; set; }
    }
}