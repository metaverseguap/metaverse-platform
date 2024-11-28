#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Пример добавления кнопки к методу.
//
// [InspectorButton(Name = "Start game")]
// public void StartGameButton()
// {
//    ...
// }

/// <summary>
/// <para>Атрибут отображаемый в инспекторе в виде кнопки.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class InspectorButtonAttribute : Attribute
{
    /// <summary>
    /// Название атрибута.
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// <para>Отрисовка кнопки в инспекторе.</para>
/// </summary>
[CustomEditor(typeof(object), true, isFallback = false)]
[CanEditMultipleObjects]
public sealed class InspectorButton : Editor
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