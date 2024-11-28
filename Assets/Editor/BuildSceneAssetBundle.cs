using Global.Bundles;
using Global.AssetPackages;
using UnityEditor;

/// <summary>
/// <para>Меню Editor-а создающее Bundle Asset для сцен.</para>
/// </summary>
public sealed class BuildSceneAssetBundle : AbstractBuildAssetBundle
{
    
    private const string WINDOW_NAME = "Собрать Bundle сцен";

    /// <summary>
    /// <para>Метод добавляющий пункт меню "Build Asset Bundle/Scene".</para>
    /// </summary>
    [MenuItem("Assets/Build Asset Bundle/Scene")]
    static void BuildAssetBundles()
    {
        window = GetWindow(typeof(BuildSceneAssetBundle));
        SetupWindow();
        window.titleContent.text = WINDOW_NAME;
    }

    /// <summary>
    /// <inheritdoc cref="AbstractBuildAssetBundle.GetAssets"/>
    /// </summary>
    protected override string[] GetAssets()
    {
        return AssetPackagesUtils.GetSceneAssets();
    }

    /// <summary>
    /// <inheritdoc cref="AbstractBuildAssetBundle.AssetBundlesPath"/>
    /// </summary>
    protected override string AssetBundlesPath()
    {
        return BundleConstants.ASSET_SCENE_BUNDLES_PATH;
    }
}