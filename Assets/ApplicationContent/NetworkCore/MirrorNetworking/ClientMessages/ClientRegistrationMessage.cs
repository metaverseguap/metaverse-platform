using Mirror;

namespace NetworkCore.MirrorNetworking.ClientMessages
{
    /// <summary>
    /// <para>Сообщение о добавлении нового клиента на сервер.</para>
    /// <remarks>Клиент не имеет прямого доступа к серверным методам и объектам.
    /// Что бы запросить у сервера вызвать какой-либо метод у себя, используются сообщения <c>NetworkMessage</c>.
    /// Сообщения регистрируются на сервере вызовом метода <c>NetworkServer.RegisterHandler</c>.
    /// Сообщения отправляются на сервер при помощи метода <c>NetworkClient.Send</c>.</remarks>
    /// </summary>
    public struct ClientRegistrationMessage : NetworkMessage
    {
        public readonly string Avatar;
        public readonly string Nickname;
        
        public ClientRegistrationMessage(string avatar, string nickname)
        {
            Avatar = avatar;
            Nickname = nickname;
        }
    }
}