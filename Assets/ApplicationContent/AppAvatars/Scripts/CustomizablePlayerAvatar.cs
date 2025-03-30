using NetworkCore.MirrorNetworking.Types.Devices;
using UnityEngine;

namespace AppAvatars
{
    /// <summary>
    /// <inheritdoc cref="AbstractPlayerAvatar"/>
    /// </summary>
    public sealed class CustomizablePlayerAvatar : AbstractPlayerAvatar
    {
        /// <summary>
        /// Устройство контролирующее игрока.
        /// </summary>
        [Tooltip("Устройство контролирующее игрока")] 
        [SerializeField] private Device _playerControlDevice;

        /// <summary>
        /// <inheritdoc cref="AbstractPlayerAvatar.PlayerControlDevice"/>
        /// </summary>
        public override Device PlayerControlDevice => _playerControlDevice;
    }
}