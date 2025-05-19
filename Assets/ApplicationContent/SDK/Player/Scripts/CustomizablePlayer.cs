using LDR.SUAI_Metaverse.SDK.Types.Devices;

using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Player
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