using System.Collections.Generic;
using UserSystem.RoleSystem.Core;

namespace UserSystem.RoleSystem.Types
{
    /// <summary>
    /// <para>Информация о роли пользователя.</para>
    /// </summary>
    public sealed class RoleInfo
    {
        /// <summary>
        /// Название роли.
        /// </summary>
        public AppRole Name { get; set; }

        /// <summary>
        /// Права роли (разрешения).
        /// </summary>
        public List<AppPermission> Permissions { get; set; }
    }
}