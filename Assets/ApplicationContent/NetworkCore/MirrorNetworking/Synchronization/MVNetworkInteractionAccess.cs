using LDR.SUAI_Metaverse.SDK.Core.Interactions;
using Mirror;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization
{
    /// <summary>
    /// <para>Сетевой компонент, проверяющий возможность взаимодействия с интерактивным объектом в данный момент.</para>
    /// </summary>
    [RequireComponent(typeof(MVNetworkOwnedObject))]
    public sealed class MVNetworkInteractionAccess : NetworkBehaviour, IInteractionAccess
    {
        [SyncVar]
        private bool hasControl = false;

        private GameObject currentController = null;

        private MVNetworkOwnedObject ownership;

        private void Start()
        {
            ownership = GetComponent<MVNetworkOwnedObject>();
        }

        /// <summary>
        /// <inheritdoc cref="IInteractionAccess.IsInteractionAllowed"/>
        /// </summary>
        /// <returns><inheritdoc cref="IInteractionAccess.IsInteractionAllowed"/></returns>
        public bool IsInteractionAllowed()
        {
            return (ownership.ObjectHasNoOwner() || ownership.AmIOwner())
                   && !hasControl;
        }

        /// <summary>
        /// <inheritdoc cref="IInteractionAccess.HasInteractionControl"/>
        /// </summary>
        /// <param name="controller"><inheritdoc cref="IInteractionAccess.HasInteractionControl"/></param>
        /// <returns><inheritdoc cref="IInteractionAccess.HasInteractionControl"/></returns>
        public bool HasInteractionControl(GameObject controller)
        {
            return ownership.AmILastOwner()
                   && hasControl
                   && currentController != null && controller == currentController;
        }

        /// <summary>
        /// <inheritdoc cref="IInteractionAccess.AcquireControl"/>
        /// </summary>
        /// <param name="controllingObject"><inheritdoc cref="IInteractionAccess.AcquireControl"/></param>
        public void AcquireControl(GameObject controllingObject)
        {
            if (controllingObject == null)
            {
                return;
            }

            ownership.CmdRequestOwnership();
            ownership.ExtendOwnership();
            currentController = controllingObject;
            CmdSetHasControl(true);
        }

        /// <summary>
        /// <inheritdoc cref="IInteractionAccess.ReleaseControl"/>
        /// </summary>
        public void ReleaseControl()
        {
            ownership.CmdStopOwnership();
            currentController = null;
            CmdSetHasControl(false);
        }

        /// <summary>
        /// <para>Метод вызываемый при потере контроля над объектом указанным игроком.</para>
        /// </summary>
        /// <param name="owner">игрок потерявший контроль над объектом</param>
        public void ReleaseControlFrom(NetworkConnectionToClient owner)
        {
            ownership.CmdStopOwnership(owner);
            if (ownership.IsLastOwnerConnectionId(owner.connectionId))
            {
                currentController = null;
                CmdSetHasControl(false);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetHasControl(bool hasControl)
        {
            this.hasControl = hasControl;
        }
    }
}