using System.Threading;
using MainMenu.Containers;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Статус игрока при подключении к сцене.</para>
    /// </summary>
    public enum PlayerConnectionStatus
    {
        None,
        Host,
        Client
    }

    /// <summary>
    /// <para>Хранилище данных необходимых для подключения игрока к сетевой сцене.</para>
    /// </summary>
    public sealed class ConnectionStore
    {
        /// <summary>
        /// Статус игрока при подключении к сцене.
        /// </summary>
        public PlayerConnectionStatus PlayerStatus { get; set; }

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
    }
}