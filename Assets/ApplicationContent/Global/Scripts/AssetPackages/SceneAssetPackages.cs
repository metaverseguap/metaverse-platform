using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Global.Converters;
using Global.Files;
using Global.Logger;
using MainMenu.Containers;
using NetworkCore.MirrorNetworking.Types.Devices;
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
            ISet<string> remoteSceneNames = new HashSet<string>();
            foreach (SceneInfo sceneInfo in serverScenesInfos)
            {
                remoteSceneNames.Add(sceneInfo.Name);
            }

            IList<SceneInfoDTO> localScenesInfos = GetSceneInfos();

            for (int i = localScenesInfos.Count - 1; i >= 0; i--)
            {
                if (!remoteSceneNames.Contains(localScenesInfos[i].name))
                {
                    AppLogger.Log($"Deleting scene {localScenesInfos[i].name} because it doesn't exist on server");
                    string sceneFilePath = Path.Combine(ASSETS_DIRECTORY, localScenesInfos[i].name);
                    FileUtils.RemoveFile(sceneFilePath);
                    FileUtils.RemoveFile(sceneFilePath + ".meta");
                    localScenesInfos.RemoveAt(i);
                }
            }

            return localScenesInfos
                .Select(ToSceneInfo)
                .ToList();
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
                
            return sceneInfo;
        }
    }
}