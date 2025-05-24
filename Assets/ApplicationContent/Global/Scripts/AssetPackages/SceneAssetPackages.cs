using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Global.Converters;
using Global.Files;
using Global.Logger;
using LDR.SUAI_Metaverse.SDK.Core.Types.Devices;
using MainMenu.Containers;
using NetworkCore.ServerInteraction.Type.Scene;
using UnityEngine;

namespace Global.AssetPackages
{
    /// <summary>
    /// <para>Методы работы с файлами ассетов сцен.</para>
    /// </summary>
    public sealed class SceneAssetPackages
    {
        /// <summary>
        /// Директория хранения ассетов.
        /// </summary>
        public static readonly string ASSETS_DIRECTORY = Path.Combine(Application.streamingAssetsPath, "Scenes");
        /// <summary>
        /// Путь до файла информации о сценах.
        /// </summary>
        public static readonly string INFO_FILE_PATH = Path.Combine(ASSETS_DIRECTORY, "scenesInfo.json");

        /// <summary>
        /// <para>Сохранить список информации о сценах в файл.</para>
        /// </summary>
        public static void SaveSceneInfo(IList<SceneInfoDTO> scenes)
        {
            AssetBundleUtils.SerializeAsset(INFO_FILE_PATH, scenes);
        }

        /// <summary>
        /// <para>Получить список информации о сценах из файла ассета.</para>
        /// </summary>
        /// <returns>список информации о сценах из файла ассета</returns>
        public static IList<SceneInfoDTO> GetSceneInfos()
        {
            return AssetBundleUtils.DeserializeAsset<IList<SceneInfoDTO>>(INFO_FILE_PATH);
        }

        /// <summary>
        /// <para>Получить локальные сцены, соответсвующие сценам сервера.</para>
        ///
        /// Если локальная сцена присутствует в списке сцен сервера, то ее не нужно повторно скачивать.
        /// Если локальная сцена отсутствует в списке сцен сервера, то она будет удалена с локальной машины
        /// 
        /// <remarks>локальные сцены отсутствующие в списке сцен сервера будут <b>УДАЛЕНЫ</b> с локальной машины</remarks>
        /// </summary>
        /// <param name="serverScenesInfos">список сцен сервера</param>
        /// <returns>список локальных сцен, имеющихся на сервере</returns>
        public static IList<SceneInfo> GetMatchingLocalScenes(IList<SceneInfo> serverScenesInfos)
        {
            IDictionary<string, SceneInfo> remoteScenes = new Dictionary<string, SceneInfo>();
            foreach (SceneInfo sceneInfo in serverScenesInfos)
            {
                remoteScenes[sceneInfo.Name] = sceneInfo;
            }

            IList<SceneInfoDTO> localScenes = GetSceneInfos();

            for (int i = localScenes.Count - 1; i >= 0; i--)
            {
                SceneInfoDTO localScene = localScenes[i];
                if (!remoteScenes.ContainsKey(localScene.name))
                {
                    AppLogger.Log($"Deleting scene {localScene.name} because it doesn't exist on server");
                    RemoveSceneFile(localScene.name);
                    localScenes.RemoveAt(i);
                    continue;
                }
                
                SceneInfo remoteAvatar = remoteScenes[localScene.name];
                if (remoteAvatar.UpdateDate != localScene.updateDate)
                {
                    AppLogger.Log($"Deleting scene {localScene.name} because it is outdated");
                    RemoveSceneFile(localScene.name);
                    localScenes.RemoveAt(i);
                }
            }

            return localScenes
                .Select(ToSceneInfo)
                .ToList();
        }

        private static void RemoveSceneFile(string sceneName)
        {
            string sceneFilePath = Path.Combine(ASSETS_DIRECTORY, sceneName);
            FileUtils.RemoveFile(sceneFilePath);
            FileUtils.RemoveFile(sceneFilePath + ".meta");
        }

        private static SceneInfo ToSceneInfo(SceneInfoDTO dto)
        {
            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.Name = dto.name;
            sceneInfo.DisplayName = dto.name;
            if (Enum.TryParse(dto.device, out Device device))
            {
                sceneInfo.Device = device;
            }
            sceneInfo.SortIndex = dto.sortIndex;
            sceneInfo.Image = DataConverter.SpriteFromRowData(dto.imageData);
            sceneInfo.UpdateDate = dto.updateDate;
                
            return sceneInfo;
        }
    }
}