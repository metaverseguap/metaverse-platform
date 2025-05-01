using LDR.SUAI_Metaverse.SDK.Interactions;
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
        [SyncVar] private bool hasControl = false;

        private GameObject currentController = null;

        private MVNetworkOwnedObject ownership;

        private void Start()
        {
            ownership = GetComponent<MVNetworkOwnedObject>();
        }

        public bool IsInteractionAllowed()
        {
            return (ownership.ObjectHasNoOwner() || ownership.AmIOwner())
                   && !hasControl;
        }

        public bool HasInteractionControl(GameObject controller)
        {
            return ownership.AmILastOwner()
                   && hasControl
                   && currentController != null && controller == currentController;
        }

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