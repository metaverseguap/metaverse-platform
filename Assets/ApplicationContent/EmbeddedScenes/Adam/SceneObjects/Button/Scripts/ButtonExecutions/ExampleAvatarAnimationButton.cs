using System.Collections.Generic;
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

        public void Interact()
        {
            // Базовая интерактивность
        }

        public void Interact(Animator owner)
        {
            // Вызов базой интерактивности
            Interact();

            // Интерактивность специфическая для вызывающего игрока
            StartCoroutine(AnimationUtils.PlayExternalAnimation(owner, AnimatorConstants.PLAYER_EXTERNAL_ANIMATION_CLIP, _playAnimations));
        }
    }
}