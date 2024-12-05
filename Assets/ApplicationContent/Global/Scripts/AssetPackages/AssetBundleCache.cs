using System.Collections.Generic;
using UnityEngine;

namespace Global.AssetPackages
{
    /// <summary>
    /// <para>Загруженные Asset Bundle.</para>
    /// </summary>
    public sealed class AssetBundleCache
    {
        private static IDictionary<string, AssetBundle> assetBundlesCache = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// <para>Добавить ассет в кэш.</para>
        /// </summary>
        /// <param name="newBundle">добавляемый ассет</param>
        /// <returns>false, если добавляемый ассет уже есть в кэше</returns>
        public static bool AddBundle(AssetBundle newBundle)
        {
            if (assetBundlesCache.ContainsKey(newBundle.name))
                return false;

            assetBundlesCache.Add(newBundle.name, newBundle);
            return true;
        }

        /// <summary>
        /// <para>Получить ассет из кэша.</para>
        /// </summary>
        /// <param name="name">имя ассета</param>
        /// <returns>ассет из кэша или null, если ассета нет в кэше</returns>
        public static AssetBundle GetBundle(string name)
        {
            assetBundlesCache.TryGetValue(name, out AssetBundle bundle);
            return bundle;
        }

        /// <summary>
        /// <para>Удалить ассет из кэша.</para>
        /// </summary>
        /// <param name="name">имя ассета</param>
        /// <returns>false, если в кэше нет указанного ассета</returns>
        public static bool RemoveBundle(string name)
        {
            if (assetBundlesCache.ContainsKey(name) == false)
                return false;

            assetBundlesCache.Remove(name);
            return true;
        }
    }
}