using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MainMenu.Containers;
using Unity.VisualScripting;

namespace NetworkCore.MirrorNetworking.Containers
{
    /// <summary>
    /// <para>Хранилище сцен.</para>
    /// </summary>
    public sealed class SceneStore
    {
        // Поле вызывается в многопоточной среде - необходима синхронизация
        private readonly IList<SceneInfo> sceneInfos = new List<SceneInfo>();
        private readonly ReaderWriterLockSlim sceneInfosLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// <para>Список всех сцен в приложении.</para>
        /// </summary>
        public IList<SceneInfo> SceneInfos
        {
            get
            {
                sceneInfosLock.EnterReadLock();
                try
                {
                    return sceneInfos.ToList();
                }
                finally
                {
                    sceneInfosLock.ExitReadLock();
                }
            }
            set
            {
                sceneInfosLock.EnterWriteLock();
                try
                {
                    sceneInfos.Clear();
                    sceneInfos.AddRange(value);
                }
                finally
                {
                    sceneInfosLock.ExitWriteLock();
                }
            }
        }

        // Поле вызывается в многопоточной среде - необходима синхронизация
        private SceneInfo currentScene;
        private readonly ReaderWriterLockSlim currentSceneLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// <para>Сцена, в которой находится пользователь.</para>
        /// </summary>
        public SceneInfo CurrentScene
        {
            get
            {
                currentSceneLock.EnterReadLock();
                try
                {
                    return currentScene.Clone();
                }
                finally
                {
                    currentSceneLock.ExitReadLock();
                }
            }
            set
            {
                currentSceneLock.EnterWriteLock();
                try
                {
                    currentScene = value?.Clone();
                }
                finally
                {
                    currentSceneLock.ExitWriteLock();
                }
            }
        }
        
        /// <summary>
        /// <para>Имя сцены главного меню.</para>
        ///
        /// После выхода из метавселенной мы должны загрузить сцену меню.
        /// Данное свойство хранит имя сцены загружаемой при отключении пользователя от метавселенной
        /// </summary>
        public string MenuSceneName { get; set; }
        
        /// <summary>
        /// <para>Имя сцены загрузки.</para>
        ///
        /// Данное имя нужно, для того что бы возвращаться в при переходе между сценами.
        /// </summary>
        public string LoadingSceneName { get; set; }
    }
}