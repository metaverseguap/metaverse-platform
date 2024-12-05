using System.IO;
using Global.Files;
using Global.Logger;
using Newtonsoft.Json;
using UnityEngine;

namespace Global.AssetPackages
{
    /// <summary>
    /// <para>Класс для работы с загружаемыми ассетами.</para>
    /// </summary>
    public static class AssetBundleUtils
    {
        
        /// <summary>
        /// <para>Сериализовать ассет в файл.</para>
        /// </summary>
        /// <param name="assetPath">путь к файлу</param>
        /// <param name="asset">сериализуемый объект</param>
        /// <typeparam name="T">тип сериализуемого объекта</typeparam>
        public static void SerializeAsset<T>(string assetPath, T asset)
        {
            const string emptyList = "[]";
            FileUtils.EnsureFileExists(assetPath, emptyList, true);
            File.WriteAllText(assetPath, JsonConvert.SerializeObject(asset, Formatting.Indented));
        }

        /// <summary>
        /// <para>Десериализовать ассет из файла.</para>
        /// </summary>
        /// <param name="assetPath">путь к файлу ассета</param>
        /// <typeparam name="R">тип объекта к которому будет преобразовано содержимое файла</typeparam>
        /// <returns>десериализованный ассет типа R</returns>
        public static R DeserializeAsset<R>(string assetPath)
        {
            const string emptyList = "[]";
            FileUtils.EnsureFileExists(assetPath, emptyList, true);
            return JsonConvert.DeserializeObject<R>(File.ReadAllText(assetPath));
        }
        
        /// <summary>
        /// <para>Получить основной Game Object из Asset Bundle.</para>
        ///
        /// <remarks>данный метод стоит применять только к Asset Bundle состоящим из одного объекта</remarks>
        /// </summary>
        /// <param name="assetBundle">Asset Bundle</param>
        /// <returns>основной Game Object или null, если не удалось получить основной Game Object из Asset Bundle</returns>
        public static GameObject GetMainGameObject(AssetBundle assetBundle)
        {
            var assetName = GetMainAssetName(assetBundle);
            return assetBundle.LoadAsset<GameObject>(assetName);
        }
        
        /// <summary>
        /// <para>Получить имя главного ассета в Asset Bundle.</para>
        ///
        /// <remarks>данный метод стоит применять только к Asset Bundle состоящим из одного объекта</remarks>
        /// </summary>
        /// <param name="assetBundle">Asset Bundle</param>
        /// <returns>имя главного ассета в Asset Bundle или null, если Asset Bundle пустой</returns>
        public static string GetMainAssetName(AssetBundle assetBundle)
        {
            var assetFormBundle = assetBundle.GetAllAssetNames();
            if (assetFormBundle != null && assetFormBundle.Length > 0)
            {
                if (assetFormBundle.Length > 1)
                {
                    AppLogger.Warning($"Multiple assets found for {assetBundle.name}");
                }
                
                string assetPath = assetFormBundle[0];
                return Path.GetFileName(assetPath);
            }

            return null;
        }
    }
}