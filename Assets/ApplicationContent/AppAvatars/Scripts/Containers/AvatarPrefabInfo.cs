using System;
using AppAvatars.Types;
using UnityEngine;

namespace AppAvatars.Containers
{
    /// <summary>
    /// <para>Контейнер информации о префабе аватара.</para>
    /// </summary>
    [Serializable]
    public sealed class AvatarPrefabInfo
    {
        /// <summary>
        /// <para>Префаб аватара.</para>
        /// 
        /// <remarks>префаб аватара игрока должен иметь аниматор, т.к. в аниматоре хранятся кости модели аватара</remarks>
        /// </summary>
        public Animator Prefab;

        /// <summary>
        /// <para>Тип контроллера анимации аватара.</para>
        /// </summary>
        public AnimationControllerType AvatarAnimationControllerType;
    }
}