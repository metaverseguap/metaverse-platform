using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Auth.Response
{
    /// <summary>
    /// <para>Объект результата запроса авторизации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class AuthResponse : ResponseDetails
    {
        /// <summary>
        /// JSON Web Token для авторизации на сервере.
        /// </summary>
        public string token { get; set; }
    }
}