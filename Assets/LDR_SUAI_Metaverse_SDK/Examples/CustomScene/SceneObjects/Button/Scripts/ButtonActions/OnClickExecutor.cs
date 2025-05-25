using LDR.SUAI_Metaverse.SDK.Core.Interactions;
using UnityEngine;
using UnityEngine.Events;

namespace CustomScene.SceneObjects.Button.ButtonActions
{
    /// <summary>
    /// <para>Компонент, вызывающий метод по нажатию на кнопку.</para>
    /// </summary>
    public sealed class OnClickExecutor : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent _onPressed;
        [Tooltip("Разрешить взаимодействие через зажатие клавиши")]
        [SerializeField] private bool _holdInteraction;

        /// <summary>
        /// <inheritdoc cref="IInteractable.AllowHoldInteraction"/>
        /// </summary>
        public bool AllowHoldInteraction => _holdInteraction;

        /// <summary>
        /// <inheritdoc cref="IInteractable.Interact()"/>
        /// </summary>
        public void Interact()
        {
            _onPressed?.Invoke();
        }
    }
}