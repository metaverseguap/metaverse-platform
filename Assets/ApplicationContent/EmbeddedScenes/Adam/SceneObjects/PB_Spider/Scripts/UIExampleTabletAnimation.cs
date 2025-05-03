using UnityEngine;

namespace EmbeddedScenes.Adam.SceneObjects.PB_Spider
{
    /// <summary>
    /// <para>Пример ui, воспроизводящего анимацию другого объекта по нажатию.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    public sealed class UIExampleTabletAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        /// <summary>
        /// <para>Начать воспроизводить анимацию.</para>
        /// </summary>
        public void StartAnimation()
        {
            _animator.SetBool("isWalking", true);
        }

        /// <summary>
        /// <para>Прекратить воспроизводить анимацию.</para>
        /// </summary>
        public void StopAnimation()
        {
            _animator.SetBool("isWalking", false);
        }
    }
}