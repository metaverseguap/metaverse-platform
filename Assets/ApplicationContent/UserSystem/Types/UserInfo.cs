using UserSystem.RoleSystem.Types;

namespace UserSystem.Types
{
    /// <summary>
    /// <para>Информация о пользователе.</para>
    /// </summary>
    public sealed class UserInfo
    {
        /// <summary>
        /// Login пользователя.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Nickname пользователя.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public RoleInfo Role { get; set; }
    }
}