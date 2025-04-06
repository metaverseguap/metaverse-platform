using System.Collections.Generic;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Animations
{
    /// <summary>
    /// <para>Компонент хранящий в себе все анимации, воспроизводимые в сети.</para>
    /// </summary>
    public static class NetworkAnimationCache
    {
        private static Dictionary<string, AnimationClip> clipCache = new Dictionary<string, AnimationClip>();

        /// <summary>
        /// <para>Добавить анимацию, воспроизводящуюся в сети, в кеш.</para>
        /// </summary>
        /// <param name="clip">добавляемая анимация</param>
        /// <returns>false, если добавляемая анимация уже есть в кеше</returns>
        public static bool AddAnimation(AnimationClip clip)
        {
            return clipCache.TryAdd(clip.name, clip);
        }

        /// <summary>
        /// <para>Получить анимацию из кеша.</para>
        /// </summary>
        /// <param name="name">имя анимации</param>
        /// <returns>AnimationClip из кеша или null, если анимации нет в кеше</returns>
        public static AnimationClip GetAnimation(string name)
        {
            return clipCache.GetValueOrDefault(name);
        }

        /// <summary>
        /// <para>Удалить анимацию из кеша.</para>
        /// </summary>
        /// <param name="name">имя анимации</param>
        /// <returns>false, если в кеше нет указанной анимации</returns>
        public static bool RemoveAnimation(string name)
        {
            if (clipCache.ContainsKey(name) == false)
            {
                return false;
            }

            clipCache.Remove(name);
            return true;
        }

        /// <summary>
        /// <para>Очистить кеш анимаций.</para>
        /// </summary>
        public static void ClearCache()
        {
            clipCache.Clear();
        }
    }
}