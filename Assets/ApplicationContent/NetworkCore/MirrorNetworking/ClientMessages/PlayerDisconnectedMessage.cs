using Mirror;

namespace NetworkCore.MirrorNetworking.ClientMessages
{
    /// <summary>
    /// <para>Сообщение, передающее клиенту данные об отключении игрока.</para>
    /// </summary>
    public struct PlayerDisconnectedMessage : NetworkMessage
    {
        /// <summary>
        /// NetId отключенного игрока.
        /// </summary>
        public readonly uint netId;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="netId">NetId отключенного игрока</param>
        public PlayerDisconnectedMessage(uint netId)
        {
            this.netId = netId;
        }
    }
}