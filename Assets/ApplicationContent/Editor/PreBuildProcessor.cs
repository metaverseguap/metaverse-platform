using UnityEditor;
using UnityEngine;

/// <summary>
/// <para>Класс выполняющий логику перед запуском play mode и перед build.</para>
/// Логика выполняется перед запуском play mode.
/// При build первая сборка отменяется,
/// затем включается play mode, что активирует логику,
/// затем play mode сразу же выключается,
/// после этого повторно выполняется build.
/// </summary>
[InitializeOnLoad]
public class PreBuildProcessor
{
    private static bool isBuildReady = false;
    private static bool needsRebuild = false;
    private static BuildPlayerOptions pendingBuildOptions;

    static PreBuildProcessor()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPressed);
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    
    private static void OnBuildPressed(BuildPlayerOptions options)
    {
        if (!isBuildReady)
        {
            Debug.Log("[BuildInterceptor] Build intercepted — project not ready.");
            pendingBuildOptions = options;
            needsRebuild = true;
            // Запускаем play mode, что бы активировать логику PreProcess
            EditorApplication.EnterPlaymode();
            return;
        }

        Debug.Log("[BuildInterceptor] Starting actual build...");
        BuildPipeline.BuildPlayer(options);
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Логика
            PreProcess();

            if (needsRebuild)
            {
                isBuildReady = true;
                needsRebuild = false;

                EditorApplication.ExitPlaymode();
            }
        }

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            if (isBuildReady)
            {
                BuildPipeline.BuildPlayer(pendingBuildOptions);

                isBuildReady = false;
            }
        }
    }

    private static void PreProcess()
    {
        NetworkPrefabProcessor.ProcessNetworkPrefabs();
    }
}