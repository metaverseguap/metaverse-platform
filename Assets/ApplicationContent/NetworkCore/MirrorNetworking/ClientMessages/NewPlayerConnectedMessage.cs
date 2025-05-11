using Mirror;

namespace NetworkCore.MirrorNetworking.ClientMessages
{
    /// <summary>
    /// <para>Сообщение, передающее клиенту данные о подключении нового игрока.</para>
    /// </summary>
    public struct NewPlayerConnectedMessage : NetworkMessage
    {
        /// <summary>
        /// NetId добавленного игрока.
        /// </summary>
        public readonly uint netId;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="netId">NetId добавленного игрока</param>
        public NewPlayerConnectedMessage(uint netId)
        {
            this.netId = netId;
        }
    }
}