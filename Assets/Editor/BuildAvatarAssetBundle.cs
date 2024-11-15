using Global.Bundles;
using Global.AssetPackages;
using UnityEditor;

/// <summary>
/// <para>Меню Editor-а создающее Bundle Asset для Аватаров.</para>
/// </summary>
public sealed class BuildAvatarAssetBundle : AbstractBuildAssetBundle
{
    private const string WINDOW_NAME = "Собрать Bundle Аватаров";

    /// <summary>
    /// <para>Метод добавляющий пункт меню "Build Asset Bundle/Avatars".</para>
    /// </summary>
    [MenuItem("Assets/Build Asset Bundle/Avatars")]
    static void BuildAssetBundles()
    {
        window = GetWindow(typeof(BuildAvatarAssetBundle));
        SetupWindow();
        window.titleContent.text = WINDOW_NAME;
    }

    /// <summary>
    /// <inheritdoc cref="AbstractBuildAssetBundle.GetAssets"/>
    /// </summary>
    protected override string[] GetAssets()
    {
        return AssetPackagesUtils.GetGameObjectAssets("avatar");
    }

    /// <summary>
    /// <inheritdoc cref="AbstractBuildAssetBundle.AssetBundlesPath"/>
    /// </summary>
    protected override string AssetBundlesPath()
    {
        return BundleConstants.ASSET_AVATAR_BUNDLES_PATH;
    }
}