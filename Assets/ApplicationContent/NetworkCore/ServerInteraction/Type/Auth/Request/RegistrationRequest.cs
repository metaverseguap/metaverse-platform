namespace NetworkCore.ServerInteraction.Type.Auth.Request
{
    /// <summary>
    /// <para>Объект запроса регистрации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class RegistrationRequest
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
        /// Повторный пароль пользователя.
        /// </summary>
        public string repeatPassword { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Ключ регистрации пользователя.
        /// </summary>
        public string registrationKey { get; set; }
    }
}