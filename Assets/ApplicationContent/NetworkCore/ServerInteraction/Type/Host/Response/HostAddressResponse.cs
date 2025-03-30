using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Host.Response
{
    /// <summary>
    /// <para>Объект результата запроса создания хоста на файловом сервере.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class HostAddressResponse : ResponseDetails
    {
        /// <summary>
        /// Адрес созданного хоста.
        /// </summary>
        public HostAddressDTO hostAddress { get; set; }
    }
}