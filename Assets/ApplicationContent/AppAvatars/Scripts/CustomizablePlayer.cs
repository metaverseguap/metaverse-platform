using NetworkCore.MirrorNetworking.Types.Devices;
using Player.EmbeddedPlayers;
using UnityEngine;

namespace AppAvatars
{
    /// <summary>
    /// <inheritdoc cref="AbstractPlayer"/>
    /// </summary>
    public sealed class CustomizablePlayer : AbstractPlayer
    {
        /// <summary>
        /// Устройство контролирующее игрока.
        /// </summary>
        [Tooltip("Устройство контролирующее игрока")] 
        [SerializeField] private Device _playerControlDevice;

        /// <summary>
        /// <inheritdoc cref="AbstractPlayer.PlayerControlDevice"/>
        /// </summary>
        public override Device PlayerControlDevice => _playerControlDevice;
    }
}