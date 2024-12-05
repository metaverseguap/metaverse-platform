using System.Collections.Generic;

namespace NetworkCore.ServerInteraction.Type.Role
{
    /// <summary>
    /// <para>Объект передачи данных роли.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class RoleDTO
    {
        /// <summary>
        /// Название роли.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Права (разрешения) роли.
        /// </summary>
        public List<PermissionDTO> permissions { get; set; }
    }
}