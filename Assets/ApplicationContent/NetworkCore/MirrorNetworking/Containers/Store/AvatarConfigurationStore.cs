using System;
using System.Collections.Generic;
using AppAvatars.Containers;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище настройки аватаров.</para>
    /// </summary>
    public sealed class AvatarConfigurationStore
    {
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="builder">builder</param>
        private AvatarConfigurationStore(AvatarConfigurationStoreBuilder builder)
        {
            AnimatorControllers = builder.AnimatorControllers;
        }

        /// <summary>
        /// Контроллеры анимации аватаров игрока.
        /// </summary>
        public List<AnimatorControllerInfo> AnimatorControllers { get; }

        /// <summary>
        /// Builder.
        /// </summary>
        public static AvatarConfigurationStoreBuilder Builder => new AvatarConfigurationStoreBuilder();

        /// <summary>
        /// Builder.
        /// </summary>
        public sealed class AvatarConfigurationStoreBuilder : IDisposable
        {
            /// <summary>
            /// Контроллеры анимации аватаров игрока.
            /// </summary>
            public List<AnimatorControllerInfo> AnimatorControllers { get; private set; }

            /// <param name="animatorControllers">контроллеры анимации аватаров игрока</param>
            /// <returns>self</returns>
            public AvatarConfigurationStoreBuilder WithAnimatorControllers(List<AnimatorControllerInfo> animatorControllers)
            {
                AnimatorControllers = animatorControllers;
                return this;
            }

            /// <summary>
            /// <para>Собрать объект класса.</para>
            /// </summary>
            /// <returns>объект класса</returns>
            public AvatarConfigurationStore Build()
            {
                return new AvatarConfigurationStore(this);
            }

            /// <summary>
            /// <para>Освобождение ресурсов GC.</para> 
            /// </summary>
            public void Dispose()
            {
                AnimatorControllers = null;
            }
        }
    }
}