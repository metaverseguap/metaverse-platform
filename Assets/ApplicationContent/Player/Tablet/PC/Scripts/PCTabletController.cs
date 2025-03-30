using Cinemachine;
using Player.EmbeddedPlayers;
using UnityEngine;

namespace Player.Tablet.PC
{
    /// <summary>
    /// <para>Компонент отвечающий за управление планшетом при помощи клавиатуры и мышки.</para>
    /// </summary>
    public sealed class PCTabletController : AbstractPlayerController
    {
        [SerializeField] private CinemachineVirtualCamera _playerCamera;

        /// <summary>
        /// <inheritdoc cref="AbstractPlayerController.DeactivatePermanently"/>
        /// </summary>
        public override void DeactivatePermanently()
        {
            base.DeactivatePermanently();
            _playerCamera.enabled = false;
        }
    }
}