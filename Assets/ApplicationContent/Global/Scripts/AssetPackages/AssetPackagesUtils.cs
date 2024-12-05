using System.Collections.Generic;
using System.Linq;
using Global.Files;
using UnityEditor;

namespace Global.AssetPackages
{
    /// <summary>
    /// <para>Набор вспомогательных методов для работы с Unity Asset Package.</para>
    /// </summary>
    public static class AssetPackagesUtils
    {
// Работа с ассетами может происходить только в редакторе Unity
// Данный код не должен попадать в сборку
#if UNITY_EDITOR
        public const string ASSET_DIRECTORY_PATH = "TemporaryAssets";
        private static readonly string[] BUNDLES_SEARCH_PATH = { "Assets\\ApplicationContent" };

        /// <summary>
        /// <para>Получить имена всех unity asset package сцен в приложении.</para>
        /// </summary>
        /// <returns>массив имен unity asset package сцен в приложении</returns>
        public static string[] GetSceneAssets()
        {
            string[] sceneList = AssetDatabase.FindAssets("t:Scene", BUNDLES_SEARCH_PATH);
            return ExtractBundleAssets(sceneList);
        }
        
        /// <summary>
        /// <para>Получить имена всех unity asset package для game object в приложении.</para>
        /// </summary>
        /// <returns>массив имен unity asset package для game object в приложении</returns>
        public static string[] GetGameObjectAssets(string label = null)
        {
            string filter = "t:Object";
            if (label != null)
            {
                filter += " l:" + label;
            }
            
            string[] objectList = AssetDatabase.FindAssets(filter, BUNDLES_SEARCH_PATH);
            return ExtractBundleAssets(objectList);
        }

        private static string[] ExtractBundleAssets(string[] names)
        {
            IList<string> assetNames = new List<string>();

            foreach (var sceneName in names)
            {
                var bundleAsset = ToBundleAsset(sceneName);

                if (bundleAsset != null && !string.IsNullOrEmpty(bundleAsset.assetBundleName))
                {
                    string assetLabel = AssetDatabase.GetImplicitAssetBundleName(bundleAsset.assetPath);
                    assetNames.Add(assetLabel);
                }
            }

            return assetNames.ToArray();
        }

        private static AssetImporter ToBundleAsset(string name)
        {
            string path = AssetDatabase.GUIDToAssetPath(name);
            return AssetImporter.GetAtPath(path);
        }

        /// <summary>
        /// <para>Проверяет существование ассета в проекте.</para>
        /// </summary>
        /// <param name="assetPath">имя ассета</param>
        /// <returns>true, если ассет с таким именем существует</returns>
        public static bool AssetExists(string assetPath)
        {
            return AssetDatabase.GUIDToAssetPath(assetPath) != null;
        }
        
        /// <summary>
        /// <para>Собрать все unity asset package в файлы.</para>
        /// </summary>
        /// <param name="outputPath">директория, в которую будут помещены созданные файлы. Если директории не существует, она будет создана</param>
        public static void BuildAllBundles(string outputPath)
        {
            FileUtils.EnsureDirectoryExists(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }

        /// <summary>
        /// <para>Собрать указанный unity asset package в файл.</para>
        /// </summary>
        /// <param name="bundleName">имя собираемого unity asset package</param>
        /// <param name="outputPath">директория, в которую будет помещен созданный файл. Если директории не существует, она будет создана</param>
        public static void BuildAssetBundleByName(string bundleName, string outputPath = ASSET_DIRECTORY_PATH)
        {
            if (string.IsNullOrEmpty(bundleName))
            {
                return;
            }

            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = assetPaths;

            AssetBundleBuild[] builds = new[] { build };

            FileUtils.EnsureDirectoryExists(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, builds, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }

        /// <summary>
        /// <para>Собрать указанные unity asset package в файлы.</para>
        /// </summary>
        /// <param name="bundleNames">имена собираемых unity asset package</param>
        /// <param name="outputPath">директория, в которую будут помещены созданные файлы. Если директории не существует, она будет создана</param>
        public static void BuildAssetBundlesByName(IList<string> bundleNames, string outputPath = ASSET_DIRECTORY_PATH)
        {
            IList<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var bundleName in bundleNames)
            {
                if (string.IsNullOrEmpty(bundleName))
                {
                    continue;
                }

                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = bundleName;
                build.assetNames = assetPaths;

                builds.Add(build);
            }

            FileUtils.EnsureDirectoryExists(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, builds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
#endif
    }
}