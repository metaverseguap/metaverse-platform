using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Host.Response
{
    /// <summary>
    /// <para>Объект результата запроса информации о хостах указанной сцены.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SinglesceneHostsResponse : ResponseDetails
    {
        /// <summary>
        /// Список хостов указанной сцены.
        /// </summary>
        public List<HostInfoDTO> hosts { get; set; }
    }
}