namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер регистрационных данных.</para>
    /// </summary>
    public sealed class RegistrationInfo
    {
        /// <summary>
        /// Login пользователя.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Повторный пароль пользователя.
        /// </summary>
        public string RepeatPassword { get; set; }

        /// <summary>
        /// Никнейм пользователя.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Ключ регистрации пользователя.
        /// </summary>
        public string RegistrationKey { get; set; }
    }
}