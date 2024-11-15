namespace NetworkCore.ServerInteraction.Type.Role
{
    /// <summary>
    /// <para>Объект роли файлового сервера полученный с сервера.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SecurityRoleRO
    {
        /// <summary>
        /// Название роли.
        /// </summary>
        public string name { get; set; }
    }
}