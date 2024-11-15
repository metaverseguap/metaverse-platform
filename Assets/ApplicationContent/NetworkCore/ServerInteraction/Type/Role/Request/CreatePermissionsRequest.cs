using System.Collections.Generic;

namespace NetworkCore.ServerInteraction.Type.Role.Request
{
    /// <summary>
    /// <para>Объект запроса создания множества прав (разрешений).</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class CreatePermissionsRequest
    {
        /// <summary>
        /// Список создаваемых прав (разрешений).
        /// </summary>
        public List<PermissionRO> permissions { get; set; }
    }
}