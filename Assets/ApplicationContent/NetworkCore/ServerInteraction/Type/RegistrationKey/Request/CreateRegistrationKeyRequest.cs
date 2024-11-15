namespace NetworkCore.ServerInteraction.Type.RegistrationKey.Request
{
    /// <summary>
    /// <para>Объект запроса создания ключа регистрации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class CreateRegistrationKeyRequest
    {
        /// <summary>
        /// Ключ регистрации.
        /// </summary>
        public RegistrationKeyRO registrationKey { get; set; }
    }
}