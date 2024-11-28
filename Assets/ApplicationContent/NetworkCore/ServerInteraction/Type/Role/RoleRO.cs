using System.Collections.Generic;

namespace NetworkCore.ServerInteraction.Type.Role
{
    /// <summary>
    /// <para>Объект роли полученный с сервера.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class RoleRO
    {
        /// <summary>
        /// Название роли.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Права (разрешения) роли.
        /// </summary>
        public List<PermissionRO> permissions { get; set; }
    }
}