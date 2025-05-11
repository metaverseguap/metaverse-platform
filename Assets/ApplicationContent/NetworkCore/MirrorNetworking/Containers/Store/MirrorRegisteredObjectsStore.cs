using System.Collections.Generic;

namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище данных о регистрируемых в Mirror объектах.</para>
    /// </summary>
    public sealed class MirrorRegisteredObjectsStore
    {
        /// <summary>
        /// <para>Хеш-коды зарегистрированных префабов.</para>
        ///
        /// Mirror не позволяет просто спавнить любые объекты в сцене.
        /// Объекты должны быть зарегистрированы в <c>NetworkManager.spawnPrefabs</c>.
        /// В данном сете хранятся хеш-коды зарегистрированных префабов,
        /// для быстрого поиска зарегистрированных префабов
        /// </summary>
        public ISet<int> RegisterPrefabsHash { get; } = new HashSet<int>();
    }
}