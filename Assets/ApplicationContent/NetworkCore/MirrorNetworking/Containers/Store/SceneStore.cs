using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MainMenu.Containers;
using Unity.VisualScripting;

namespace NetworkCore.MirrorNetworking.Containers.Store
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
    }
}