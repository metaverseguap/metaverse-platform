namespace NetworkCore.ServerInteraction.Type.Auth.Request
{
    /// <summary>
    /// <para>Объект запроса авторизации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class LoginRequest
    {
        /// <summary>
        /// Login пользователя.
        /// </summary>
        public string login { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// Ключ авторизации пользователя.
        ///
        /// <remarks>Не обязателен для суперпользователей.</remarks>
        /// </summary>
        public string loginKey { get; set; }
    }
}