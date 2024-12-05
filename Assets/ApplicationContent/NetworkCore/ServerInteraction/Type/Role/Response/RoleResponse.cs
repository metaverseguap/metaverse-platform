using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Role.Response
{
    /// <summary>
    /// <para>Объект результата запроса получения роли.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class RoleResponse : ResponseDetails
    {
        /// <summary>
        /// Информация о роли.
        /// </summary>
        public RoleDTO roleInfo { get; set; }
    }
}