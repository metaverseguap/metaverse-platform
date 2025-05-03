using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppAvatars.Containers;
using MainMenu.Containers;
using Unity.VisualScripting;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище аватаров.</para>
    /// </summary>
    public sealed class AvatarStore
    {
        // Поле вызывается в многопоточной среде - необходима синхронизация
        private readonly IList<AvatarInfo> avatarInfos = new List<AvatarInfo>();
        private readonly ReaderWriterLockSlim avatarInfosLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// <para>Информация о всех аватарах хранящихся на файловом сервере.</para>
        /// </summary>
        public IList<AvatarInfo> AvatarInfos
        {
            get
            {
                avatarInfosLock.EnterReadLock();
                try
                {
                    return avatarInfos.ToList();
                }
                finally
                {
                    avatarInfosLock.ExitReadLock();
                }
            }
            set
            {
                avatarInfosLock.EnterWriteLock();
                try
                {
                    avatarInfos.Clear();
                    avatarInfos.AddRange(value);
                }
                finally
                {
                    avatarInfosLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// <para>Файлы аватаров.</para>
        ///
        /// Ключом служит имя файла
        /// </summary>
        public IDictionary<string, AvatarFile> AvatarFiles { get;} = new Dictionary<string, AvatarFile>();
        
        /// <summary>
        /// Контроллеры анимации аватаров игрока.
        /// </summary>
        public List<AnimatorControllerInfo> AnimatorControllers { set; get; }
    }
}