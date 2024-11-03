#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Example of adding button.
//
// [InspectorButton(Name = "Start game")]
// public void StartGameButton()
// {
//    ...
// }

/// <summary>
/// <para>Attribute displayed in the inspector as a button.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class InspectorButtonAttribute : Attribute
{
    private string name;

    /// <summary>
    /// The attribute name.
    /// </summary>
    public string Name
    {
        get => name;
        set => name = value;
    }
}

/// <summary>
/// <para>Draw button in inspector.</para>
/// </summary>
[CustomEditor(typeof(object), true, isFallback = false)]
[CanEditMultipleObjects]
sealed class InspectorButton : Editor
{
    /// <summary>
    /// <inheritdoc cref="Editor.OnInspectorGUI"/>
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        foreach (var t in targets)
        {
            var methodInfos =
                t.GetType().GetMethods()
                    .Where(method => method.GetCustomAttributes()
                        .Any(attribute => attribute is InspectorButtonAttribute)
                    );

            foreach (var method in methodInfos)
            {
                InspectorButtonAttribute buttonAttribute = method.GetCustomAttribute<InspectorButtonAttribute>();
                if (GUILayout.Button(buttonAttribute.Name))
                {
                    method?.Invoke(t, null);
                }
            }
        }
    }
}
#endif