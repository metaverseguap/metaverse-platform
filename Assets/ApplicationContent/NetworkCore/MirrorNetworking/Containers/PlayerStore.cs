using AppAvatars;
using NetworkCore.MirrorNetworking.Player.Base;

namespace NetworkCore.MirrorNetworking.Containers
{
    /// <summary>
    /// <para>Статус игрока при подключении к сцене.</para>
    /// </summary>
    public enum PlayerConnectionStatus
    {
        None,
        Host,
        Client
    }
    
    /// <summary>
    /// <para>Хранилище данных об игроке.</para>
    /// </summary>
    public sealed class PlayerStore
    {
        /// <summary>
        /// Префаб сетевого игрока для текущей сборки.
        /// </summary>
        public NetworkBasePlayer NetworkPlayer { get;  set; }
        
        /// <summary>
        /// Префаб игрока для текущей сборки.
        /// </summary>
        public AbstractPlayer CurrentBuildPlayer { get; set; }
        
        /// <summary>
        /// Имя аватара игрока.
        /// </summary>
        public string AvatarName { get; set; }
        
        /// <summary>
        /// Статус игрока при подключении к сцене.
        /// </summary>
        public PlayerConnectionStatus PlayerStatus { get; set; }
    }
}