using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Role.Response
{
    /// <summary>
    /// <para>Объект результата запроса множества ролей файлового сервера.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class ServerRolesResponse : ResponseDetails
    {
        /// <summary>
        /// Роли файлового сервера.
        /// </summary>
        public List<SecurityRoleDTO> roles { get; set; }
    }
}