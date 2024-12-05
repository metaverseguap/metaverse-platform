namespace NetworkCore.ServerInteraction.Type.Role
{
    /// <summary>
    /// <para>Объект передачи данных роли файлового сервера.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SecurityRoleDTO
    {
        /// <summary>
        /// Название роли.
        /// </summary>
        public string name { get; set; }
    }
}