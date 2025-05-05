using System;
using AppAvatars.Types;
using UnityEngine;

namespace AppAvatars.Containers
{
    /// <summary>
    /// <para>Контейнер информации об контроллере анимации.</para>
    /// </summary>
    [Serializable]
    public sealed class AnimatorControllerInfo
    {
        /// <summary>
        /// Контроллер анимации.
        /// </summary>
        public AnimatorOverrideController Controller;

        /// <summary>
        /// Тип анимации, которой управляет данный контроллер.
        /// </summary>
        public AnimationControllerType AnimationControllerType;
    }
}