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
        /// Пол анимациями которого управляет данный контроллер.
        /// </summary>
        public AnimationControllerType AnimationControllerType;
    }
}