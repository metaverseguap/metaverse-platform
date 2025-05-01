using System.Collections.Generic;
using System.IO;
using Global.AssetPackages;
using Global.Bundles;
using Global.Files;
using Mirror;
using NetworkCore.MirrorNetworking.Offline;
using NetworkCore.MirrorNetworking.Synchronization;
using NetworkCore.MirrorNetworking.Synchronization.Animations;
using NetworkCore.MirrorNetworking.Synchronization.Transforms;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <para>Методы обработки сетевых префабов.</para>
/// </summary>
public static class NetworkPrefabProcessor
{
    private const string APPLICATION_CONTENT_PATH = "Assets/ApplicationContent";

    /// <summary>
    /// <para>Интегрировать внешние префабы, имеющие <see cref="NetworkObject"/>, в сетевую структуру проекта.</para>
    /// Дополняет все префабы, имеющие <see cref="NetworkObject"/>, необходимыми сетевыми компонентами.
    /// Обновляет данные префабы в сценах.
    /// Пересобирает bundle сцен с обновленными префабами.
    /// </summary>
    public static void ProcessNetworkPrefabs()
    {
        ISet<GameObject> modifiedPrefabs = ProcessPrefabs();
        UpdateScenesWithPrefabs(modifiedPrefabs);
    }

    private static ISet<GameObject> ProcessPrefabs()
    {
        string[] prefabAssets = AssetDatabase.FindAssets("t:Prefab", new[] { APPLICATION_CONTENT_PATH });
        ISet<GameObject> modifiedPrefabs = new HashSet<GameObject>();

        foreach (string prefabAsset in prefabAssets)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabAsset);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                continue;
            }

            NetworkObject networkObject = prefab.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                continue;
            }

            bool isUpdated = UpdatePrefab(prefab, networkObject, path);
            if (isUpdated)
            {
                modifiedPrefabs.Add(prefab);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return modifiedPrefabs;
    }

    private static bool UpdatePrefab(GameObject prefab, NetworkObject networkObject, string path)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        bool updated = false;

        if (networkObject.SyncObject)
        {
            if (instance.GetComponent<OfflineModeObject>() == null)
            {
                Debug.Log($"Add OfflineModeObject to {prefab}");
                instance.AddComponent<OfflineModeObject>();
                updated = true;
            }
            
            if (instance.GetComponent<NetworkIdentity>() == null)
            {
                Debug.Log($"Add NetworkIdentity to {prefab}");
                instance.AddComponent<NetworkIdentity>();
                updated = true;
            }
        }

        if (networkObject.AnimatedObject && instance.GetComponent<MVNetworkAnimator>() == null)
        {
            Debug.Log($"Add MVNetworkAnimator to {path}");
            AnimatorParameterListener parameterListener = instance.AddComponent<AnimatorParameterListener>();
            MVNetworkAnimator networkAnimator = instance.AddComponent<MVNetworkAnimator>();
            networkAnimator.enabled = false;
            
            networkAnimator.ParameterListener = parameterListener;

            networkAnimator.enabled = true;
            
            updated = true;
        }

        if (networkObject.TransformObject
            && instance.GetComponent<MVNetworkTransform>() == null)
        {
            // Не допускаем одновременной синхронизации Transform и Rigidbody
            MVNetworkRigidBody rigidBodySync = instance.GetComponent<MVNetworkRigidBody>();
            GameObject.DestroyImmediate(rigidBodySync);
            
            Debug.Log($"Add MVNetworkTransform to {path}");
            MVNetworkTransform networkTransform = instance.AddComponent<MVNetworkTransform>();
            networkTransform.Target = networkObject.transform;
            

            
            updated = true;
        }

        if (networkObject.PhysicObject
            && instance.GetComponent<MVNetworkRigidBody>() == null)
        {
            // Не допускаем одновременной синхронизации Transform и Rigidbody
            MVNetworkTransform transformSync = instance.GetComponent<MVNetworkTransform>();
            GameObject.DestroyImmediate(transformSync);
            
            Debug.Log($"Add MVNetworkRigidBody to {path}");
            MVNetworkRigidBody networkRigidbody = instance.AddComponent<MVNetworkRigidBody>();
            networkRigidbody.Target = networkObject.GetComponent<Rigidbody>();
            
            updated = true;
        }

        if (networkObject.CanBeOwned
            && instance.GetComponent<MVNetworkInteractionAccess>() == null)
        {
            Debug.Log($"Add MVNetworkInteractionAccess to {prefab}");
            instance.AddComponent<MVNetworkInteractionAccess>();
            updated = true;
        }

        if (updated)
        {
            PrefabUtility.SaveAsPrefabAsset(instance, path);
            Debug.Log($"Update prefab: {path}");
        }

        GameObject.DestroyImmediate(instance);

        return updated;
    }

    private static void UpdateScenesWithPrefabs(ISet<GameObject> modifiedPrefabs)
    {
        if (modifiedPrefabs == null || modifiedPrefabs.Count == 0)
        {
            return;
        }

        string[] sceneAssets = AssetDatabase.FindAssets("t:Scene", new[] { APPLICATION_CONTENT_PATH });

        foreach (string sceneAsset in sceneAssets)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneAsset);
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            bool isSceneModified = SceneContainsModifiedPrefab(scene, modifiedPrefabs);
            if (isSceneModified)
            {
                EditorSceneManager.SaveScene(scene);
            }

            // Закрываем, чтобы не трогать редактор
            EditorSceneManager.CloseScene(scene, true);

            if (isSceneModified)
            {
                RecreateSceneBundle(sceneAsset);
            }
        }
    }

    private static bool SceneContainsModifiedPrefab(Scene scene, ISet<GameObject> modifiedPrefabs)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                GameObject instance = child.gameObject;
                GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(instance);
                if (source != null && modifiedPrefabs.Contains(source))
                {
                    Debug.Log($"[Scene Fix] Updating scene '{scene.path}' due to prefab '{source.name}'");
                    EditorSceneManager.MarkSceneDirty(scene);
                    return true;
                }
            }
        }

        return false;
    }

    private static void RecreateSceneBundle(string sceneAsset)
    {
        string assetLabel = AssetPackagesUtils.GetBundleLabel(sceneAsset);
        AssetPackagesUtils.BuildAssetBundleByName(assetLabel, BundleConstants.ASSET_SCENE_BUNDLES_PATH);
        FileUtils.RemoveFile(Path.Combine(SceneAssetPackages.ASSETS_DIRECTORY, assetLabel));
        FileUtils.RemoveFile(Path.Combine(SceneAssetPackages.ASSETS_DIRECTORY, $"{assetLabel}.meta"));
        FileUtils.RemoveFile(SceneAssetPackages.INFO_FILE_PATH);
    }
}