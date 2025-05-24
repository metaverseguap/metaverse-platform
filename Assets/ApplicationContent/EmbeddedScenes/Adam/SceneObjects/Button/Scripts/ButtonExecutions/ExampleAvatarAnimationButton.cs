using System.Collections.Generic;
using LDR.SUAI_Metaverse.SDK.Core.Animations;
using LDR.SUAI_Metaverse.SDK.Core.Interactions;
using UnityEngine;

namespace Adam.SceneObjects.Button.ButtonExecution
{
    /// <summary>
    /// <para>Пример кнопки, воспроизводящей анимацию по нажатию.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    public sealed class ExampleAvatarAnimationButton : MonoBehaviour, IInteractable
    {
        [Tooltip("Анимация воспроизводимая игроком после нажатия на кнопку")] 
        [SerializeField] private List<AnimationClip> _playAnimations;

        /// <summary>
        /// <inheritdoc cref="IInteractable.Interact()"/>
        /// </summary>
        public void Interact()
        {
            // Базовая интерактивность
        }

        /// <summary>
        /// <inheritdoc cref="IInteractable.Interact(GameObject)"/>
        /// </summary>
        /// <param name="owner"><inheritdoc cref="IInteractable.Interact(GameObject)"/></param>
        public void Interact(GameObject owner)
        {
            // Вызов базой интерактивности
            Interact();

            // Интерактивность специфическая для вызывающего игрока
            if (owner.TryGetComponent(out Animator playerAnimator))
            {
                StartCoroutine(AnimationUtils.PlayExternalAnimation(playerAnimator, _playAnimations));
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}]: Not found animator in interactor '{owner.name}'.");
            }
        }
    }
}