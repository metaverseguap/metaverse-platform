using System.Collections.Generic;
using UnityEngine;

namespace Global.AssetPackages
{
    /// <summary>
    /// <para>Загруженные Asset Bundle.</para>
    /// </summary>
    public static class AssetBundleCache
    {
        private static IDictionary<string, AssetBundle> assetBundlesCache = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// <para>Добавить ассет в кеш.</para>
        /// </summary>
        /// <param name="newBundle">добавляемый ассет</param>
        /// <returns>false, если добавляемый ассет уже есть в кеше</returns>
        public static bool AddBundle(AssetBundle newBundle)
        {
            return assetBundlesCache.TryAdd(newBundle.name, newBundle);
        }

        /// <summary>
        /// <para>Получить ассет из кеша.</para>
        /// </summary>
        /// <param name="name">имя ассета</param>
        /// <returns>ассет из кеша или null, если ассета нет в кеше</returns>
        public static AssetBundle GetBundle(string name)
        {
            assetBundlesCache.TryGetValue(name, out AssetBundle bundle);
            return bundle;
        }

        /// <summary>
        /// <para>Удалить ассет из кеша.</para>
        /// </summary>
        /// <param name="name">имя ассета</param>
        /// <returns>false, если в кеше нет указанного ассета</returns>
        public static bool RemoveBundle(string name)
        {
            if (assetBundlesCache.ContainsKey(name) == false)
            {
                return false;
            }

            assetBundlesCache.Remove(name);
            return true;
        }

        /// <summary>
        /// <para>Очистить кеш.</para>
        /// </summary>
        public static void ClearCache()
        {
            assetBundlesCache.Clear();
        }
    }
}