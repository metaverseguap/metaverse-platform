using System.Collections;
using LDR.SUAI_Metaverse.SDK.Interactions;
using UnityEngine;

namespace Adam.SceneObjects.Button.ButtonActions
{
    /// <summary>
    /// <para>Анимация кнопки при взаимодействии с кнопкой</para>
    /// </summary>
    public sealed class PressButtonAnimation : MonoBehaviour, IInteractable
    {
        private static readonly int IS_ACTIVE = Animator.StringToHash("isActive");
        private static readonly int HOLD_IT_PRESSED = Animator.StringToHash("holdItPressed");
        private const float RELEASE_DELAY = 0.05f; // FixedUpdate вызывается раз в 0.02f

        [SerializeField] private Animator _buttonAnimator;
        [Tooltip("Зажимать кнопку при зажатии взаимодействия")]
        [SerializeField] private bool _holdAnimation;
        
        private Coroutine _releaseCoroutine;
        
        public bool AllowHoldInteraction => _holdAnimation;

        public void Interact()
        {
            // Базовая интерактивность
            _buttonAnimator.SetTrigger(IS_ACTIVE);

            if (_holdAnimation)
            {
                _buttonAnimator.SetBool(HOLD_IT_PRESSED, true);
                
                if (_releaseCoroutine != null)
                {
                    StopCoroutine(_releaseCoroutine);
                }
                
                _releaseCoroutine = StartCoroutine(ReleaseHoldAfterDelay());
            }
        }

        private IEnumerator ReleaseHoldAfterDelay()
        {
            yield return new WaitForSeconds(RELEASE_DELAY);

            _buttonAnimator.SetBool(HOLD_IT_PRESSED, false);
            _releaseCoroutine = null;
        }
    }
}