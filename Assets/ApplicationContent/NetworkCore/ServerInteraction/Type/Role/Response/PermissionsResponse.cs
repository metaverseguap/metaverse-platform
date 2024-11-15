using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Role.Response
{
    /// <summary>
    /// <para>Объект результата запроса получения прав (разрешений).</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class PermissionsResponse : ResponseDetails
    {
        /// <summary>
        /// Права (разрешения).
        /// </summary>
        public List<PermissionRO> permissions { get; set; }
    }
}