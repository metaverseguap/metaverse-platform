namespace NetworkCore.ServerInteraction.Type.LoginKey.Request
{
    /// <summary>
    /// <para>Объект запроса создания ключа авторизации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class CreateLoginKeyRequest
    {
        /// <summary>
        /// Ключ авторизации.
        /// </summary>
        public LoginKeyDTO loginKey { get; set; }
    }
}