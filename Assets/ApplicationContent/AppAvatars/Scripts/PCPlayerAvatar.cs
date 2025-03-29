using NetworkCore.MirrorNetworking.Types.Devices;

namespace AppAvatars
{
    /// <summary>
    /// <para>Класс игрока управляемого с компьютера.</para>
    /// </summary>
    public sealed class PCPlayerAvatar : AbstractPlayerAvatar
    {
        /// <summary>
        /// <inheritdoc cref="AbstractPlayerAvatar.PlayerControlDevice"/>
        /// </summary>
        public override Device PlayerControlDevice => Device.PC;
    }
}