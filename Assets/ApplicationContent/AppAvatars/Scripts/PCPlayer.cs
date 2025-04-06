using NetworkCore.MirrorNetworking.Types.Devices;
using Player.EmbeddedPlayers;

namespace AppAvatars
{
    /// <summary>
    /// <para>Класс игрока управляемого с компьютера.</para>
    /// </summary>
    public sealed class PCPlayer : AbstractPlayer
    {
        /// <summary>
        /// <inheritdoc cref="AbstractPlayer.PlayerControlDevice"/>
        /// </summary>
        public override Device PlayerControlDevice => Device.PC;
    }
}