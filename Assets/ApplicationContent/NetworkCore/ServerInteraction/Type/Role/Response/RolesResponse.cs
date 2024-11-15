using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Role.Response
{
    /// <summary>
    /// <para>Объект результата запроса получения множества ролей.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class RolesResponse : ResponseDetails
    {
        /// <summary>
        /// Роли.
        /// </summary>
        public List<RoleRO> roles { get; set; }
    }
}