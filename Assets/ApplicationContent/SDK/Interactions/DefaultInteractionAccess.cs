using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Interactions
{
    /// <summary>
    /// <para>Компонент по умолчанию, проверяющий возможность взаимодействия с интерактивным объектом в данный момент.</para>
    /// </summary>
    public sealed class DefaultInteractionAccess : MonoBehaviour, IInteractionAccess
    {
        [SerializeField] private bool _allowInteraction = true;

        private bool hasControl = false;
        private GameObject currentController = null;

        /// <summary>
        /// <inheritdoc cref="IInteractionAccess.IsInteractionAllowed"/>
        /// </summary>
        /// <returns><inheritdoc cref="IInteractionAccess.IsInteractionAllowed"/></returns>
        public bool IsInteractionAllowed()
        {
            return _allowInteraction && !hasControl;
        }

        /// <summary>
        /// <inheritdoc cref="IInteractionAccess.HasInteractionControl"/>
        /// </summary>
        /// <param name="controller"><inheritdoc cref="IInteractionAccess.HasInteractionControl"/></param>
        /// <returns><inheritdoc cref="IInteractionAccess.HasInteractionControl"/></returns>
        public bool HasInteractionControl(GameObject controller)
        {
            return hasControl
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

            currentController = controllingObject;
            hasControl = true;
        }

        /// <summary>
        /// <inheritdoc cref="IInteractionAccess.ReleaseControl"/>
        /// </summary>
        public void ReleaseControl()
        {
            currentController = null;
            hasControl = false;
        }
    }
}