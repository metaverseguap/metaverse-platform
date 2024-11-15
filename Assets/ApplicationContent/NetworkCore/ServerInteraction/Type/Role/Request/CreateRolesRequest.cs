using System.Collections.Generic;

namespace NetworkCore.ServerInteraction.Type.Role.Request
{
    /// <summary>
    /// <para>Объект запроса создания множества ролей.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class CreateRolesRequest
    {
        /// <summary>
        /// Список создаваемых ролей.
        /// </summary>
        public List<RoleRO> roles { get; set; }
    }
}