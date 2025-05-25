using LDR.SUAI_Metaverse.SDK.Core.Interactions;
using UnityEngine;

namespace Adam.SceneObjects.Button.ButtonExecution
{
    /// <summary>
    /// <para>Пример кнопки, воспроизводящей анимацию другого объекта по нажатию.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    public sealed class ExampleAnimateOtherObjectButton : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator _animator;

        /// <summary>
        /// <inheritdoc cref="IInteractable.Interact()"/>
        /// </summary>
        public void Interact()
        {
            // Базовая интерактивность
            _animator.SetTrigger("jump");
        }
    }
}