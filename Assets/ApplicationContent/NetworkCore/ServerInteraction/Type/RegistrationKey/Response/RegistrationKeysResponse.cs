using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.RegistrationKey.Response
{
    /// <summary>
    /// <para>Ответ на запрос получения множества ключей регистрации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class RegistrationKeysResponse : ResponseDetails
    {
        /// <summary>
        /// Ключи регистрации.
        /// </summary>
        public List<RegistrationKeyRO> keys { get; set; }
    }
}