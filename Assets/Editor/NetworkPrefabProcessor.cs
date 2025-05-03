using System.Collections.Generic;
using System.IO;
using Global.AssetPackages;
using Global.Bundles;
using Global.Files;
using LDR.SUAI_Metaverse.SDK.NetworkSync;
using Mirror;
using NetworkCore.MirrorNetworking.Offline;
using NetworkCore.MirrorNetworking.Synchronization;
using NetworkCore.MirrorNetworking.Synchronization.Animations;
using NetworkCore.MirrorNetworking.Synchronization.Transforms;
using NetworkCore.MirrorNetworking.Synchronization.UI;
using TMPro;
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

        updated = SyncIdentity(networkObject, path, instance);

        updated = SyncAnimation(networkObject, path, instance) || updated;

        updated = SyncTransform(networkObject, path, instance) || updated;

        updated = SyncRigidBody(networkObject, path, instance) || updated;

        updated = SyncAccess(prefab, networkObject, instance) || updated;

        updated = SyncUI(prefab, networkObject, instance) || updated;

        if (updated)
        {
            PrefabUtility.SaveAsPrefabAsset(instance, path);
            Debug.Log($"Update prefab: {path}");
        }

        GameObject.DestroyImmediate(instance);

        return updated;
    }

    private static bool SyncIdentity(NetworkObject networkObject, string path, GameObject instance)
    {
        bool updated = false;
        if (networkObject.SyncObject)
        {
            if (instance.GetComponent<OfflineModeObject>() == null)
            {
                Debug.Log($"Add OfflineModeObject to {path}");
                instance.AddComponent<OfflineModeObject>();
                updated = true;
            }

            if (instance.GetComponent<NetworkIdentity>() == null)
            {
                Debug.Log($"Add NetworkIdentity to {path}");
                instance.AddComponent<NetworkIdentity>();
                updated = true;
            }
        }

        return updated;
    }

    private static bool SyncAnimation(NetworkObject networkObject, string path, GameObject instance)
    {
        bool updated = false;
        if (networkObject.AnimatedObject
            && instance.GetComponent<MVNetworkAnimator>() == null)
        {
            Debug.Log($"Add MVNetworkAnimator to {path}");
            AnimatorParameterListener parameterListener = instance.AddComponent<AnimatorParameterListener>();
            MVNetworkAnimator networkAnimator = instance.AddComponent<MVNetworkAnimator>();
            networkAnimator.enabled = false;

            networkAnimator.ParameterListener = parameterListener;

            networkAnimator.enabled = true;

            updated = true;
        }

        return updated;
    }

    private static bool SyncTransform(NetworkObject networkObject, string path, GameObject instance)
    {
        bool updated = false;
        if (networkObject.TransformObject
            && instance.GetComponent<MVNetworkTransform>() == null)
        {
            // Не допускаем одновременной синхронизации Transform и Rigidbody
            MVNetworkRigidBody rigidBodySync = instance.GetComponent<MVNetworkRigidBody>();
            GameObject.DestroyImmediate(rigidBodySync);

            Debug.Log($"Add MVNetworkTransform to {path}");
            MVNetworkTransform networkTransform = instance.AddComponent<MVNetworkTransform>();
            networkTransform.enabled = false;
            
            networkTransform.Target = networkObject.transform;
            
            networkTransform.enabled = true;
            
            updated = true;
        }

        return updated;
    }

    private static bool SyncRigidBody(NetworkObject networkObject, string path, GameObject instance)
    {
        bool updated = false;
        if (networkObject.PhysicObject
            && instance.GetComponent<MVNetworkRigidBody>() == null)
        {
            // Не допускаем одновременной синхронизации Transform и Rigidbody
            MVNetworkTransform transformSync = instance.GetComponent<MVNetworkTransform>();
            GameObject.DestroyImmediate(transformSync);

            Debug.Log($"Add MVNetworkRigidBody to {path}");
            MVNetworkRigidBody networkRigidbody = instance.AddComponent<MVNetworkRigidBody>();
            networkRigidbody.enabled = false;
            
            networkRigidbody.Target = networkObject.GetComponent<Rigidbody>();
            
            networkRigidbody.enabled = true;

            updated = true;
        }

        return updated;
    }

    private static bool SyncAccess(GameObject prefab, NetworkObject networkObject, GameObject instance)
    {
        bool updated = false;
        if (networkObject.CanBeOwned
            && instance.GetComponent<MVNetworkInteractionAccess>() == null)
        {
            Debug.Log($"Add MVNetworkInteractionAccess to {prefab}");
            instance.AddComponent<MVNetworkInteractionAccess>();
            updated = true;
        }

        return updated;
    }

    private static bool SyncUI(GameObject prefab, NetworkObject networkObject, GameObject instance)
    {
        bool updated = false;
        if (networkObject.IsCanvas
            && instance.GetComponent<MVNetworkUI>() == null)
        {
            Debug.Log($"Add MVNetworkUI to {prefab}");
            MVNetworkUI networkUI = instance.AddComponent<MVNetworkUI>();
            networkUI.enabled = false;
            
            Canvas uiParent = networkUI.gameObject.GetComponent<Canvas>();

            List<Component> components = new List<Component>();
            components.AddRange(uiParent.GetComponentsInChildren<TMP_Text>(true));

            Dictionary<Canvas, List<Component>> canvasComponents = ComponentsGroupedByCanvas(components);

            if (canvasComponents.TryGetValue(uiParent, out var canvasChildren))
            {
                List<TMP_Text> texts = new List<TMP_Text>();
                foreach (var component in canvasChildren)
                {
                    if (component.TryGetComponent(out TMP_Text textComponent))
                    {
                        texts.Add(textComponent);
                    }
                }

                networkUI.Texts = texts;
            }
            
            networkUI.enabled = true;

            updated = true;
        }

        return updated;
    }

    private static Dictionary<Canvas, List<Component>> ComponentsGroupedByCanvas(List<Component> components)
    {
        var uniqueTransforms = UniqueTransforms(components);

        var result = new Dictionary<Canvas, List<Component>>();
        foreach (var currentUi in uniqueTransforms)
        {
            Canvas parentCanvas = GetParentCanvas(currentUi);
            if (parentCanvas == null)
            {
                Debug.LogWarning($"Canvas not found parent canvas for: {currentUi.gameObject.name}");
                continue;
            }

            if (!result.ContainsKey(parentCanvas))
            {
                result.Add(parentCanvas, new List<Component>());
            }

            result[parentCanvas].Add(currentUi);
        }

        return result;
    }

    private static HashSet<Transform> UniqueTransforms(List<Component> components)
    {
        var uniqueTransforms = new HashSet<Transform>();
        foreach (var component in components)
        {
            uniqueTransforms.Add(component.transform);
        }

        return uniqueTransforms;
    }

    private static Canvas GetParentCanvas(Transform currentTransform)
    {
        while (currentTransform != null)
        {
            Canvas canvas = currentTransform.GetComponent<Canvas>();
            if (canvas != null)
            {
                return canvas;
            }

            currentTransform = currentTransform.parent;
        }

        return null;
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