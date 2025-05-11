using System;
using System.Collections.Generic;
using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Utils
{
    /// <summary>
    /// <para>Набор вспомогательных методов для работы со сценами.</para>
    /// </summary>
    public static class SceneUtils
    {
        private static float sceneStartTime = 0f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            sceneStartTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// <para>Получить время прошедшее с загрузки текущей сцены.</para>
        /// </summary>
        /// <returns>время прошедшее с загрузки текущей сцены</returns>
        public static TimeSpan GetSceneLifetime()
        {
            return TimeSpan.FromSeconds(Time.realtimeSinceStartup - sceneStartTime);
        }

        /// <summary>
        /// <para>Получить путь до указанного объекта на сцене от корневого объекта.</para>
        /// </summary>
        /// <param name="target">объект, до которого необходимо получить путь</param>
        /// <param name="root">корневой объект, если не указан то корневым объектом будет считаться корень сцены</param>
        /// <returns>путь от корневого объекта до указанного или null, если указанный объект не является потомком корневого</returns>
        public static string GetObjectPathFromRoot(Transform target, Transform root = null)
        {
            if (target == null)
            {
                return null;
            }

            List<string> pathElements = new List<string>();
            Transform current = target;

            while (current != null && current != root)
            {
                pathElements.Insert(0, current.name);
                current = current.parent;
            }

            if (root != null && current != root)
            {
                return null;
            }

            if (root == null && current != null)
            {
                return null;
            }

            return string.Join("/", pathElements);
        }
    }
}