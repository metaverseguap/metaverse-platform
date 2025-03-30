using Mirror;
using NetworkCore.MirrorNetworking.Types.Client;

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
        /// <summary>
        /// <inheritdoc cref="ClientSpawnData"/>
        /// </summary>
        public readonly ClientSpawnData SpawnData;
        
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="spawnData"><see cref="ClientSpawnData"/></param>
        public ClientRegistrationMessage(ClientSpawnData spawnData)
        {
            SpawnData = spawnData;
        }
    }
}