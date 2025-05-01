using System;
using UnityEngine;

namespace MV.SDK.Utils
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
    }
}