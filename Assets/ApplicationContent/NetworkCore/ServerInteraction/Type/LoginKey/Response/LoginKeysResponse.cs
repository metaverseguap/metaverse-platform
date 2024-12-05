using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.LoginKey.Response
{
    /// <summary>
    /// <para>Ответ на запрос получения множества ключей авторизации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class LoginKeysResponse : ResponseDetails
    {
        /// <summary>
        /// Ключи авторизации.
        /// </summary>
        public List<LoginKeyDTO> keys { get; set; }
    }
}