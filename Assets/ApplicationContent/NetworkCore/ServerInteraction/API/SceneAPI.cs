using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Global.Converters;
using Global.Files;
using Global.Logger;
using MainMenu.Containers;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.Devices;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;
using NetworkCore.ServerInteraction.Type.Scene.Response;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с api/scenes файлового сервера.</para>
    /// </summary>
    public sealed class SceneAPI : AbstractServerAPI
    {
        private const string ALL_SCENE_INFO_URL = "/api/scenes/all-info";
        private const string UPLOAD_SCENE_URL = "/api/scenes/upload-scene";
        private const string DELETE_SCENES_URL = "/api/scenes/delete-by-names";
        
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public SceneAPI(string serverUri) : base(serverUri)
        {
        }
        
        /// <summary>
        /// <para>Получает информацию о всех сценах хранящихся на файловом сервере.</para>
        ///
        /// Данный метод не подгружает сами файлы сцен, а только <see cref="SceneInfo">информацию</see> о них
        /// </summary>
        /// <returns>информация о всех сценах файлового сервера</returns>
        public IList<SceneInfo> GetAllSceneInfo()
        {
            IList<SceneInfo> result = new List<SceneInfo>();

            SceneInfosResponse response = restAPI.GetRequest<SceneInfosResponse>(ALL_SCENE_INFO_URL);

            if (response.success)
            {
                foreach (var infoRO in response.infoList)
                {
                    SceneInfo info = new SceneInfo();
                    info.Name = infoRO.name;
                    info.DisplayName = infoRO.displayName;
                    if (Enum.TryParse(infoRO.device, out Device device))
                    {
                        info.Device = device;
                    }
                    info.SortIndex = infoRO.sortIndex;
                    info.Image = DataConverter.SpriteFromRowData(infoRO.imageData);
                    
                    result.Add(info);
                }

                return result;
            }
            else
            {
                AppLogger.Error($"Scene info request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return result;
        }

        /// <summary>
        /// <para>Загружает список сцен на файловый сервер.</para>
        /// </summary>
        /// <param name="uploadingScenes">список загружаемых сцен</param>
        /// <param name="removeAfterUpload">true, если после загрузки файлов на сервер их нужно удалить из приложения</param>
        /// <returns>true, если хотя бы одна сцена была загружена на сервер</returns>
        public bool UploadScenes(IList<UploadingSceneInfo> uploadingScenes, bool removeAfterUpload = false)
        {
            bool result = false;
            foreach (var scene in uploadingScenes)
            {
                bool success = UploadScene(scene);
                result = result || success;
                if (removeAfterUpload)
                {
                    FileUtils.RemoveFile(scene.SceneFilePath);
                    FileUtils.RemoveFile(scene.SceneFilePath + ".manifest");
                }
            }
            
            return result;
        }

        private bool UploadScene(UploadingSceneInfo scene)
        {
            var formData = new MultipartFormDataContent();
            
            formData.Add(new StringContent(scene.DisplayName), "displayName");
            formData.Add(new StringContent(scene.Device.ToString()), "device");
            formData.Add(new StringContent(scene.SortIndex.ToString()), "sortIndex");
            
            byte[] imgFileData = DataConverter.SpriteToRowData(scene.Image);
            var imgFileContent = new ByteArrayContent(imgFileData);
            imgFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            formData.Add(imgFileContent, "img", scene.Image.name);
            
            byte[] sceneFileData = File.ReadAllBytes(scene.SceneFilePath);
            var sceneFileContent = new ByteArrayContent(sceneFileData);
            sceneFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            formData.Add(sceneFileContent, "scene", scene.Name);

            ResponseDetails response = restAPI.PostMultipartRequest<ResponseDetails>(UPLOAD_SCENE_URL, formData);
            
            if (!response.success)
            {
                AppLogger.Warning($"Upload Scenes request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// <para>Удаляет множество сцен.</para>
        /// </summary>
        /// <param name="deletedNames">список удаляемых сцен</param>
        /// <returns>true, если удаление прошло успешно</returns>
        public bool DeleteManyScenes(List<string> deletedNames)
        {
            DeleteByNamesRequest request = new DeleteByNamesRequest()
            {
                names = deletedNames
            };
            
            ResponseDetails response =
                restAPI.PostRequest<DeleteByNamesRequest, ResponseDetails>(DELETE_SCENES_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Delete Scenes request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }
    }
}