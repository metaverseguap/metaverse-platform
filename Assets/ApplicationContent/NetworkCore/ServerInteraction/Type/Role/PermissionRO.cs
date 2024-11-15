namespace NetworkCore.ServerInteraction.Type.Role
{
    /// <summary>
    /// <para>Права (разрешения) пользователя полученный с сервера.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class PermissionRO
    {
        /// <summary>
        /// Название права (разрешения).
        /// </summary>
        public string name { get; set; }
    }
}