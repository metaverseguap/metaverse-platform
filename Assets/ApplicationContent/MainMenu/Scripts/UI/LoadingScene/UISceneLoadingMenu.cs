using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Global.AssetPackages;
using Global.Converters;
using Global.Logger;
using Localization;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Utils;
using NetworkCore.ServerInteraction.API;
using NetworkCore.ServerInteraction.Type.Scene;
using OfflineScene;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu.UI.LoadingScene
{
    /// <summary>
    /// <para>Скрипт загрузки сцен с файлового сервера.</para>
    /// </summary>
    public sealed class UISceneLoadingMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text _loadingText;

        private MVNetworkManager connection;
        private NetworkDataStore store;
        private APIContainer serverApi;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _loadingText.text = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading");
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            connection = MVNetworkManager.singleton;
            store = connection.NetworkStore;
            serverApi = store.FileServer;

            // Загрузка ассетов с сервера в асинхронном режиме
            StartCoroutine(DownloadScene());
        }

        private IEnumerator ExecuteThenAwaitFrame(Action callback = null)
        {
            callback?.Invoke();

            // Ждем один кадр, что бы сцена загрузилась
            yield return null;
        }

        private IEnumerator DownloadScene()
        {
            // Ждем один кадр, что бы сцена загрузилась
            yield return null;
            
            // 1. Подключение к офлайн сцене
            if (OfflineSceneConnection())
            {
                yield break;
            }
            
            SceneInfoDTO downloadedScene = null;
            SceneInfo loadingSceneInfo = store.Connection.CurrentScene;
            
            SceneInfo serverScene = serverApi.Scene.GetSceneInfo(loadingSceneInfo.Name);
            SceneInfoDTO localScene = GetLocalSceneInfo(loadingSceneInfo);
            if (localScene != null && serverScene.UpdateDate == localScene.updateDate)
            {
                // 2.1.1. Загрузка сцены из кеша
                if (TryLoadSceneFromCache())
                {
                    yield break;
                }

                // 2.1.2. Загрузка файла сцены с сервера
                string loadingText = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading.scenes");
                yield return ExecuteThenAwaitFrame(() => _loadingText.text = loadingText);

                Task<SceneInfoDTO> asyncDownload = SceneDownloading();
                yield return new WaitUntil(() => asyncDownload.IsCompleted);
                downloadedScene = asyncDownload.Result;
                
                if (downloadedScene == null)
                {
                    SceneInfo loadingScene = store.Connection.CurrentScene;
                    AppLogger.Error($"Failed to download scene {loadingScene.Name} from server");
                    connection.StartOfflineScene();
                    yield break;
                }
                
                // 2.1.3. Загрузка Bundle
                if (TryConnectByBundleCache(downloadedScene))
                {
                    yield break;
                }
            }
            else
            {
                // 2.2.1. Сбрасываем кеш
                ClearSceneCache(loadingSceneInfo, serverScene.UpdateDate);

                // 2.2.2. Загрузка файла сцены с сервера
                string loadingText = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading.scenes");
                yield return ExecuteThenAwaitFrame(() => _loadingText.text = loadingText);

                Task<SceneInfoDTO> asyncReDownload = SceneReDownloading();
                yield return new WaitUntil(() => asyncReDownload.IsCompleted);
                downloadedScene = asyncReDownload.Result;
                
                if (downloadedScene == null)
                {
                    SceneInfo loadingScene = store.Connection.CurrentScene;
                    AppLogger.Error($"Failed to download scene {loadingScene.Name} from server");
                    connection.StartOfflineScene();
                    yield break;
                }
            }
            
            yield return StartCoroutine(LoadBundleToCache(downloadedScene));
            
            AssetBundle assetBundle = AssetBundleCache.GetBundle(downloadedScene.name);
            if (assetBundle == null)
            {
                AppLogger.Error($"Failed to get scene bundle {downloadedScene.name} from cache");
                connection.StartOfflineScene();
                yield break;
            }
            
            ConnectToSceneByPath(assetBundle);
        }

        private bool OfflineSceneConnection()
        {
            SceneInfo loadingScene = store.Connection.CurrentScene;
            if (loadingScene.Name == OfflineSceneConstants.SCENE_INFO.Name)
            {
                connection.ConnectToNetwork();
                return true;
            }
            
            return false;
        }
        
        private static SceneInfoDTO GetLocalSceneInfo(SceneInfo loadingSceneInfo)
        {
            IList<SceneInfoDTO> localScenes = SceneAssetPackages.GetSceneInfos();
            return localScenes.SingleOrDefault(scene => scene.name == loadingSceneInfo.Name);
        }
        
        private bool TryLoadSceneFromCache()
        {
            SceneInfo loadingScene = store.Connection.CurrentScene;

            // Path устанавливается при создании Bundle
            // если он сохранен в store, значит Bundle уже загружался
            if (loadingScene.CachedPath != null)
            {
                connection.ConnectToNetwork();
                return true;
            }

            return false;
        }

        private async Task<SceneInfoDTO> SceneDownloading()
        {
            SceneInfo loadingScene = store.Connection.CurrentScene;
            IList<SceneInfoDTO> localScenes = SceneAssetPackages.GetSceneInfos();

            SceneInfoDTO existScene = localScenes.SingleOrDefault(scene => scene.name == loadingScene.Name);

            if (existScene == default)
            {
                existScene = await DownloadSceneFile(loadingScene, localScenes);
            }

            return existScene;
        }

        private async Task<SceneInfoDTO> SceneReDownloading()
        {
            SceneInfo loadingScene = store.Connection.CurrentScene;
            IList<SceneInfoDTO> localScenes = SceneAssetPackages.GetSceneInfos();

            for (int i = localScenes.Count - 1; i >= 0; i--)
            {
                if (localScenes[i].name == loadingScene.Name)
                {
                    localScenes.RemoveAt(i);
                    break;
                }
            }

            return await DownloadSceneFile(loadingScene, localScenes);
        }

        private async Task<SceneInfoDTO> DownloadSceneFile(SceneInfo loadingScene, IList<SceneInfoDTO> localScenes)
        {
            bool success = await serverApi.Scene.GetSceneFile(loadingScene);
            if (success)
            {
                return SaveSceneInfo(loadingScene, localScenes);
            }

            AppLogger.Error($"Failed to download scene file {loadingScene.Name}");
            return null;
        }
        
        private static SceneInfoDTO SaveSceneInfo(SceneInfo loadingScene, IList<SceneInfoDTO> localScenes)
        {
            SceneInfoDTO newScene = new SceneInfoDTO();
            newScene.name = loadingScene.Name;
            newScene.displayName = loadingScene.DisplayName;
            newScene.device = loadingScene.Device.ToString();
            newScene.imageData = DataConverter.SpriteToRowData(loadingScene.Image);
            newScene.updateDate = loadingScene.UpdateDate;

            IList<SceneInfoDTO> allScenes = new List<SceneInfoDTO>();

            foreach (SceneInfoDTO scene in localScenes)
            {
                if (scene.name != newScene.name)
                {
                    allScenes.Add(scene);
                }
            }

            allScenes.Add(newScene);

            SceneAssetPackages.SaveSceneInfo(allScenes);
            return newScene;
        }

        private bool TryConnectByBundleCache(SceneInfoDTO scene)
        {
            AssetBundle assetBundle = AssetBundleCache.GetBundle(scene.name);
            if (assetBundle != null)
            {
                ConnectToSceneByPath(assetBundle);
                return true;
            }

            return false;
        }
        
        private void ClearSceneCache(SceneInfo loadingSceneInfo, DateTime updateDate)
        {
            SceneInfo newSceneCache = 
                loadingSceneInfo
                    .WithCachedPath(null)
                    .WithUpdateDate(updateDate);

            UpdateSceneCache(newSceneCache);
            RemoveAssetBundle(store.Connection.CurrentScene.Name);
        }
        
        private static void RemoveAssetBundle(string bundleName)
        {
            AssetBundle cachedBundle = AssetBundleCache.GetBundle(bundleName);
            if (cachedBundle != null)
            {
                cachedBundle.Unload(true);
                AssetBundleCache.RemoveBundle(bundleName); 
            }
        }

        private void ConnectToSceneByPath(AssetBundle assetBundle)
        {
            SceneInfo loadingScene = store.Connection.CurrentScene;
            string[] paths = assetBundle.GetAllScenePaths();

            if (paths.Length == 0)
            {
                AppLogger.Error($"Failed to load AssetBundle path for scene: {loadingScene.Name}");
                connection.StartOfflineScene();
                return;
            }

            CacheScenePath(paths);

            connection.ConnectToNetwork();
        }

        private void CacheScenePath(string[] paths)
        {
            SceneInfo loadingScene = store.Connection.CurrentScene;

            string newScenePath = paths[0];
            SceneInfo newSceneCache = loadingScene.WithCachedPath(newScenePath);

            UpdateSceneCache(newSceneCache);
        }

        private void UpdateSceneCache(SceneInfo newSceneCache)
        {
            store.Connection.CurrentScene = newSceneCache;

            IList<SceneInfo> scenesCache = store.FileStore.Scenes.SceneInfos;
            foreach (SceneInfo scene in scenesCache)
            {
                if (scene.Name == newSceneCache.Name)
                {
                    scene.CachedPath = newSceneCache.CachedPath;
                    break;
                }
            }

            store.FileStore.Scenes.SceneInfos = scenesCache;
        }

        private IEnumerator LoadBundleToCache(SceneInfoDTO scene)
        {
            SceneInfo loadingScene = store.Connection.CurrentScene;
            string bundlePath = Path.Combine(SceneAssetPackages.ASSETS_DIRECTORY, scene.name);
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(bundlePath);

            // Ожидание загрузки AssetBundle
            while (!createRequest.isDone)
            {
                yield return ExecuteThenAwaitFrame(() => UpdateLoadingText(createRequest));
            }

            // Получаем загруженный AssetBundle
            AssetBundle assetBundle = createRequest.assetBundle;

            // Проверка успешности загрузки
            if (assetBundle == null)
            {
                AppLogger.Error($"Failed to load AssetBundle {loadingScene.Name}");
                connection.StartOfflineScene();
                yield break;
            }

            // Добавляем AssetBundle в кеш
            AssetBundleCache.AddBundle(assetBundle);
        }

        private void UpdateLoadingText(AssetBundleCreateRequest createRequest)
        {
            int progress = Mathf.RoundToInt(createRequest.progress * 100);
            string assetsLoadingText = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.label.loading.scenes.assets");
            _loadingText.text = string.Format(assetsLoadingText, progress);
        }
    }
}