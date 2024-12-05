using NetworkCore.ServerInteraction.Type.Role;

namespace NetworkCore.ServerInteraction.Type.User
{
    /// <summary>
    /// <para>Объект передачи данных информации о пользователе.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class UserInfoDTO
    {
        /// <summary>
        /// Login пользователя.
        /// </summary>
        public string login { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public RoleDTO role { get; set; }
    }
}