using AppAvatars;
using NetworkCore.MirrorNetworking.Player.Base;

namespace NetworkCore.MirrorNetworking.Containers
{
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
    }
}