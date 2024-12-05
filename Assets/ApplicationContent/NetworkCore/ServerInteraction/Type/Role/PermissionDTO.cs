namespace NetworkCore.ServerInteraction.Type.Role
{
    /// <summary>
    /// <para>Объект передачи данных права (разрешения) пользователя.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class PermissionDTO
    {
        /// <summary>
        /// Название права (разрешения).
        /// </summary>
        public string name { get; set; }
    }
}