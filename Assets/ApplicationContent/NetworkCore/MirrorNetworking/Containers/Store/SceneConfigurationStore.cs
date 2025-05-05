using System;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище настройки сцен.</para>
    /// </summary>
    public sealed class SceneConfigurationStore
    {
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="builder">builder</param>
        private SceneConfigurationStore(SceneConfigurationStoreBuilder builder)
        {
            MenuSceneName = builder.MenuSceneName;
            LoadingSceneName = builder.LoadingSceneName;
        }

        /// <summary>
        /// <para>Имя сцены главного меню.</para>
        ///
        /// После выхода из метавселенной мы должны загрузить сцену меню.
        /// Данное свойство хранит имя сцены загружаемой при отключении пользователя от метавселенной
        /// </summary>
        public string MenuSceneName { get; }

        /// <summary>
        /// <para>Имя сцены загрузки.</para>
        ///
        /// Данное имя нужно, для того что бы возвращаться в при переходе между сценами.
        /// </summary>
        public string LoadingSceneName { get; }

        /// <summary>
        /// Builder.
        /// </summary>
        public static SceneConfigurationStoreBuilder Builder => new SceneConfigurationStoreBuilder();

        /// <summary>
        /// Builder.
        /// </summary>
        public sealed class SceneConfigurationStoreBuilder : IDisposable
        {
            /// <summary>
            /// <para>Имя сцены главного меню.</para>
            ///
            /// После выхода из метавселенной мы должны загрузить сцену меню.
            /// Данное свойство хранит имя сцены загружаемой при отключении пользователя от метавселенной
            /// </summary>
            public string MenuSceneName { get; private set; }

            /// <summary>
            /// <para>Имя сцены загрузки.</para>
            ///
            /// Данное имя нужно, для того что бы возвращаться в при переходе между сценами.
            /// </summary>
            public string LoadingSceneName { get; private set; }

            /// <summary>
            /// <para>Имя сцены главного меню.</para>
            ///
            /// После выхода из метавселенной мы должны загрузить сцену меню.
            /// Данное свойство хранит имя сцены загружаемой при отключении пользователя от метавселенной
            /// </summary>
            /// <param name="menuSceneName">имя сцены главного меню</param>
            /// <returns>self</returns>
            public SceneConfigurationStoreBuilder WithMenuSceneName(string menuSceneName)
            {
                MenuSceneName = menuSceneName;
                return this;
            }

            /// <summary>
            /// <para>Имя сцены загрузки.</para>
            ///
            /// Данное имя нужно, для того что бы возвращаться в при переходе между сценами.
            /// </summary>
            /// <param name="loadingSceneName">имя сцены загрузки</param>
            /// <returns>self</returns>
            public SceneConfigurationStoreBuilder WithLoadingSceneName(string loadingSceneName)
            {
                LoadingSceneName = loadingSceneName;
                return this;
            }

            /// <summary>
            /// <para>Собрать объект класса.</para>
            /// </summary>
            /// <returns>объект класса</returns>
            public SceneConfigurationStore Build()
            {
                return new SceneConfigurationStore(this);
            }

            /// <summary>
            /// <para>Освобождение ресурсов GC.</para> 
            /// </summary>
            public void Dispose()
            {
                MenuSceneName = null;
                LoadingSceneName = null;
            }
        }
    }
}