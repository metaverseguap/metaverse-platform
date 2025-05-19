using UnityEditor;
using UnityEngine;

/// <summary>
/// <para>Скрипт, создающий в стороннем приложении необходимые Layer для работы SDK.</para>
/// </summary>
[InitializeOnLoad]
public sealed class LayerSetup
{
    private static readonly string[] REQUIRED_LAYERS = { "Avatar", "Not rendered" };

    static LayerSetup()
    {
        AddRequiredLayers();
    }

    private static void AddRequiredLayers()
    {
        Object[] tagManagers = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        SerializedObject tagManager = new SerializedObject(tagManagers[0]);
        SerializedProperty layersProperty = tagManager.FindProperty("layers");

        foreach (string layer in REQUIRED_LAYERS)
        {
            if (!LayerExists(layersProperty, layer))
            {
                bool added = AddLayer(layersProperty, layer);
                if (added)
                {
                    Debug.Log($"[Digital reality lab SDK] Add new layer: {layer}");
                }
                else
                {
                    Debug.LogWarning($"[Digital reality lab SDK] Failed to add layer '{layer}' - all slots are occupied.");
                }
            }
        }

        tagManager.ApplyModifiedProperties();
    }

    private static bool LayerExists(SerializedProperty layersProp, string layerName)
    {
        for (int i = 0; i < layersProp.arraySize; i++)
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (sp != null && sp.stringValue == layerName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool AddLayer(SerializedProperty layersProp, string layerName)
    {
        // Первые 8 слотов системные
        for (int i = 8; i < layersProp.arraySize; i++)
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (sp != null && string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = layerName;
                return true;
            }
        }

        return false;
    }
}