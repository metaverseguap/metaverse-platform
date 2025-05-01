using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LDR.SUAI_Metaverse.SDK.Animations
{
    /// <summary>
    /// <para>Набор вспомогательных методов для работы с анимациями.</para>
    /// </summary>
    public static class AnimationUtils
    {
        /// <summary>
        /// Событие для начала синхронизации анимаций.
        /// </summary>
        public static UnityEvent<Animator, string, List<AnimationClip>> OnExternalAnimationStarted = new UnityEvent<Animator, string, List<AnimationClip>>();

        /// <summary>
        /// Событие для окончания синхронизации анимаций.
        /// </summary>
        public static UnityEvent<Animator> OnExternalAnimationEnded = new UnityEvent<Animator>();

        /// <summary>
        /// <para>Воспроизвести анимацию для указанного аниматора.</para>
        /// <para>
        /// В передаваемом аниматоре должно быть состояние заглушка для внешних анимаций.
        /// У этого состояния должна быть анимация имеющая то же имя, что и состояние
        /// (по умолчанию при перетаскивании клипа в аниматор создается состояние с именем этого клипа).
        /// </para>
        /// <para>
        /// Метод вызывается через корутину <c>StartCoroutine(AnimationUtils.PlayExternalAnimation(animator, externalAnimationPlaceholderName, clip));</c>.
        /// Если нужно дождаться завершения всех анимаций, то воспользуйтесь <c>yield return AnimationUtils.PlayExternalAnimation(animator, externalAnimationPlaceholderName, clipsChain);</c>
        /// внутри другой корутины
        /// </para>
        /// </summary>
        /// <param name="animator">аниматор, который будет воспроизводить анимации</param>
        /// <param name="externalAnimationPlaceholderName">имя состояния заглушки внешних анимации. Оно должно совпадать с именем клипа, который находится в этом состоянии</param>
        /// <param name="clip">воспроизводимая анимация</param>
        public static IEnumerator PlayExternalAnimation(Animator animator, string externalAnimationPlaceholderName, AnimationClip clip)
        {
            yield return PlayExternalAnimation(animator, externalAnimationPlaceholderName, new List<AnimationClip> { clip });
        }

        /// <summary>
        /// <para>Воспроизвести для указанного аниматора цепочку анимаций.</para>
        /// <para>
        /// В передаваемом аниматоре должно быть состояние заглушка для внешних анимаций.
        /// У этого состояния должна быть анимация имеющая то же имя, что и состояние
        /// (по умолчанию при перетаскивании клипа в аниматор создается состояние с именем этого клипа).
        /// </para>
        /// <para>
        /// Метод вызывается через корутину <c>StartCoroutine(AnimationUtils.PlayExternalAnimation(animator, externalAnimationPlaceholderName, clipsChain));</c>.
        /// Если нужно дождаться завершения всех анимаций, то воспользуйтесь <c>yield return AnimationUtils.PlayExternalAnimation(animator, externalAnimationPlaceholderName, clipsChain);</c>
        /// внутри другой корутины
        /// </para>
        /// </summary>
        /// <param name="animator">аниматор, который будет воспроизводить анимации</param>
        /// <param name="externalAnimationPlaceholderName">имя состояния заглушки внешних анимации. Оно должно совпадать с именем клипа, который находится в этом состоянии</param>
        /// <param name="clipsChain">цепочка воспроизводимых анимаций</param>
        public static IEnumerator PlayExternalAnimation(Animator animator, string externalAnimationPlaceholderName, List<AnimationClip> clipsChain)
        {
            if (clipsChain == null || clipsChain.Count == 0 || animator == null)
            {
                yield break;
            }

            OnExternalAnimationStarted?.Invoke(animator, externalAnimationPlaceholderName, clipsChain);

            var runtimeAnimatorController = animator.runtimeAnimatorController;
            if (runtimeAnimatorController != null && runtimeAnimatorController is AnimatorOverrideController overrideController)
            {
                foreach (AnimationClip clipToPlay in clipsChain)
                {
                    overrideController[externalAnimationPlaceholderName] = clipToPlay;

                    animator.Play(externalAnimationPlaceholderName);

                    yield return new WaitForSeconds(clipToPlay.length);
                }
            }

            OnExternalAnimationEnded?.Invoke(animator);
        }
    }
}