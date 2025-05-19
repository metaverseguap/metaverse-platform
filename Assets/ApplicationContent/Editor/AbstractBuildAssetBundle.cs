using System;
using Global.GUI_DIY;
using Global.AssetPackages;
using Global.Logger;
using UnityEditor;
using UnityEngine;

/// <summary>
/// <para>Меню Editor-а создающее Bundle Asset.</para>
///
/// Данный абстрактный класс задает общий вид для меню,
/// а конкретные реализации данного класса будут заполнять меню контентом.
/// </summary>
public abstract class AbstractBuildAssetBundle : EditorWindow
{
    private const string LIST_NAME = "Найденные Bundle";
    protected const string BASE_WINDOW_NAME = "Собрать Bundle";
    
    protected static EditorWindow window;
    private static GUISelectableList bundlesList;

    /// <summary>
    /// GUI прокручиваемый список с бандлами.
    /// </summary>
    private GUISelectableList BundlesList
    {
        get
        {
            if (bundlesList == null)
            {
                bundlesList = CreateAssetsList();
            }

            return bundlesList;
        }
    }

    /// <summary>
    /// <para>Метод создающий окно Editor-а.</para>
    /// </summary>
    protected static void SetupWindow()
    {
        window.titleContent.text = BASE_WINDOW_NAME;
        window.maxSize = new Vector2(285, 300);
        window.minSize = new Vector2(280, 290);
    }
    
    private void OnDestroy()
    {
        bundlesList = null;
    }
    
    private void OnGUI()
    {
        DrawBundleList();
        DrawCreateBundleButton();
    }

    private GUISelectableList CreateAssetsList()
    {
        GUISelectableList selectableList = new GUISelectableList(280, 200);
        selectableList.Title = LIST_NAME;
        selectableList.Subtitle = $"Путь сохранения: {AssetBundlesPath()}"; 
        selectableList.Items = GetAssets();

        return selectableList;
    }

    private void DrawBundleList()
    {
        BundlesList.Items = GetAssets();
        BundlesList.Draw(new Vector2(0, 0));
    }

    private void DrawCreateBundleButton()
    {
        const string buttonName = "Создать Bundle";
        if (GUI.Button(new Rect(0, 260, 280, 20), buttonName))
        {
            string bundleName = BundlesList.SelectedItem;
            AssetPackagesUtils.BuildAssetBundleByName(bundleName, AssetBundlesPath());
            AppLogger.Log($"Создан bundle для {bundleName}");
        }
    }
    
    /// <summary>
    /// <para>Метод получения ассетов.</para>
    ///
    /// Данный метод должен вернуть ассеты, которыми будет заполнено окно editor-а
    /// </summary>
    /// <returns>массив ассетов</returns>
    protected abstract string[] GetAssets();
    
    /// <summary>
    /// <para>Метод получения пути хранения собранных бандлов.</para>
    ///
    /// Данный метод должен вернуть путь до папки, в которой будут храниться собранные бандлы
    /// </summary>
    /// <returns>путь до папки, в которой будут храниться собранные бандлы</returns>
    protected abstract string AssetBundlesPath();
}