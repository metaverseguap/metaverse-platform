using System.Collections;
using LDR.SUAI_Metaverse.SDK.Core.Interactions;
using UnityEngine;

namespace CustomScene.SceneObjects.Button.ButtonActions
{
    /// <summary>
    /// <para>Анимация кнопки при взаимодействии с ней.</para>
    /// </summary>
    public sealed class PressButtonAnimation : MonoBehaviour, IInteractable
    {
        private static readonly int IS_ACTIVE = Animator.StringToHash("isActive");
        private static readonly int HOLD_IT_PRESSED = Animator.StringToHash("holdItPressed");
        private const float RELEASE_DELAY = 0.05f; // FixedUpdate вызывается раз в 0.02f

        [SerializeField] private Animator _buttonAnimator;
        [Tooltip("Зажимать кнопку при зажатии взаимодействия")]
        [SerializeField] private bool _holdAnimation;
        
        private Coroutine releaseCoroutine;
        
        /// <summary>
        /// <inheritdoc cref="IInteractable.AllowHoldInteraction"/>
        /// </summary>
        public bool AllowHoldInteraction => _holdAnimation;

        /// <summary>
        /// <inheritdoc cref="IInteractable.Interact()"/>
        /// </summary>
        public void Interact()
        {
            // Базовая интерактивность
            _buttonAnimator.SetTrigger(IS_ACTIVE);

            if (_holdAnimation)
            {
                _buttonAnimator.SetBool(HOLD_IT_PRESSED, true);
                
                if (releaseCoroutine != null)
                {
                    StopCoroutine(releaseCoroutine);
                }
                
                releaseCoroutine = StartCoroutine(ReleaseHoldAfterDelay());
            }
        }

        private IEnumerator ReleaseHoldAfterDelay()
        {
            yield return new WaitForSeconds(RELEASE_DELAY);

            _buttonAnimator.SetBool(HOLD_IT_PRESSED, false);
            releaseCoroutine = null;
        }
    }
}