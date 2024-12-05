using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.User.Response
{
    /// <summary>
    /// <para>Объект результата запроса информации о пользователе.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class UserInfoResponse : ResponseDetails
    {
        /// <summary>
        /// Информация о пользователе.
        /// </summary>
        public UserInfoDTO userInfo { get; set; }
    }
}