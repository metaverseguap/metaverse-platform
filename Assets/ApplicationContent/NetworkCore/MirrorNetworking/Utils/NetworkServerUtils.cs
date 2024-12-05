using Mirror;
using NetworkCore.MirrorNetworking.Player.Base;

namespace NetworkCore.MirrorNetworking.Utils
{
    /// <summary>
    /// <para>Набор вспомогательных методов для работы с NetworkServer.</para>
    /// </summary>
    public sealed class NetworkServerUtils : NetworkBehaviour
    {
        /// <summary>
        /// <para>Приватный конструктор.</para>
        /// </summary>
        private NetworkServerUtils()
        {
        }

        /// <summary>
        /// <para>Изменить префаб контролируемый игроком.</para>
        /// </summary>
        /// <param name="conn">подключение игрока</param>
        /// <param name="newPrefab">новый префаб</param>
        /// <param name="playerName">имя игрока</param>
        /// <param name="avatarName">имя аватара игрока</param>
        public static void SwapPlayerPrefab(NetworkConnectionToClient conn, NetworkBasePlayer newPrefab, string playerName, string avatarName = "")
        {
            var basePlayerInstance = Instantiate(newPrefab);
            basePlayerInstance.SetDisplayName(playerName);
            basePlayerInstance.SetAvatarName(avatarName);
            basePlayerInstance.SetConnectedId(conn.connectionId);
                
            NetworkServer.Destroy(conn.identity.gameObject);
            
            NetworkServer.ReplacePlayerForConnection(conn, basePlayerInstance.gameObject, ReplacePlayerOptions.Destroy);
        }
    }
}