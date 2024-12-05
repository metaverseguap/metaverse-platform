namespace NetworkCore.ServerInteraction.Type.Role.Request
{
    /// <summary>
    /// <para>Объект запроса создания или обновления роли.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class CreateRoleRequest
    {
        /// <summary>
        /// Роль.
        /// </summary>
        public RoleDTO role { get; set; }
    }
}