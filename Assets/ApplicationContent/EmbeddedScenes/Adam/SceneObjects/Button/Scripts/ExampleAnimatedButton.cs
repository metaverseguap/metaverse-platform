using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Adam.SceneObjects.Button
{
    /// <summary>
    /// <para>Пример кнопки, воспроизводящей анимацию по нажатию.</para>
    /// </summary>
    public sealed class ExampleAnimatedButton : MonoBehaviour, IInteractable, ITooltip
    {
        private static readonly int IS_ACTIVE = Animator.StringToHash("isActive");
        
        [SerializeField] private Animator _buttonAnimator;
        [SerializeField] private TMP_Text _buttonTooltip;
        [Tooltip("Анимация воспроизводимая игроком после нажатия на кнопку")]
        [SerializeField] private List<AnimationClip> _playAnimations;
        
        public void Interact()
        {
            // Базовая интерактивность
            _buttonAnimator.SetTrigger(IS_ACTIVE);
        }
        
        public void Interact(Animator owner)
        {
            // Вызов базой интерактивности
            Interact();
            
            // Интерактивность специфическая для вызывающего игрока
            StartCoroutine(AnimationUtils.PlayExternalAnimation(owner, AnimatorConstants.PLAYER_EXTERNAL_ANIMATION_CLIP, _playAnimations));
        }

        public void ShowTooltip()
        {
            _buttonTooltip.gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            _buttonTooltip.gameObject.SetActive(false);
        }
    }
}