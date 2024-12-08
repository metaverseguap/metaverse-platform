using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AppAvatars.Types;
using Global.AssetPackages;
using Global.Converters;
using Global.Files;
using Global.Logger;
using MainMenu.Containers;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.Avatar.Response;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с /api/avatar файлового сервера.</para>
    /// </summary>
    public sealed class AvatarAPI : AbstractServerAPI
    {
        private const string ALL_AVATAR_INFO_URL = "/api/avatar/all-info";
        private const string UPLOAD_AVATAR_URL = "/api/avatar/upload-avatar";
        private const string DELETE_AVATARS_URL = "/api/avatar/delete-by-names";
        private const string AVATAR_FILE_URL = "/api/avatar/file";

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public AvatarAPI(string serverUri) : base(serverUri)
        {
        }

        /// <summary>
        /// <para>Получает информацию о всех аватарах хранящихся на файловом сервере.</para>
        ///
        /// Данный метод не подгружает сами файлы аватаров, а только <see cref="AvatarInfo">информацию</see> о них
        /// </summary>
        /// <returns>информация о всех аватарах файлового сервера</returns>
        public IList<AvatarInfo> GetAllAvatarsInfo()
        {
            IList<AvatarInfo> result = new List<AvatarInfo>();

            AvatarInfosResponse response = restAPI.GetRequest<AvatarInfosResponse>(ALL_AVATAR_INFO_URL);

            if (response.success)
            {
                foreach (var infoRO in response.infoList)
                {
                    AvatarInfo info = new AvatarInfo();
                    info.Name = infoRO.name;
                    info.DisplayName = infoRO.displayName;
                    if (Enum.TryParse(infoRO.gender, out Gender avatarGender))
                    {
                        info.AvatarGender = avatarGender;
                    }
                    info.Image = DataConverter.SpriteFromRowData(infoRO.imageData);

                    result.Add(info);
                }

                return result;
            }
            else
            {
                AppLogger.Error($"Avatar info request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return result;
        }

        /// <summary>
        /// <para>Загружает список аватаров на файловый сервер.</para>
        /// </summary>
        /// <param name="uploadingAvatars">список загружаемых аватаров</param>
        /// <param name="removeAfterUpload">true, если после загрузки файлов на сервер их нужно удалить из приложения</param>
        /// <returns>true, если хотя бы один аватар был загружен на сервер</returns>
        public bool UploadAvatars(IList<UploadAvatarInfo> uploadingAvatars, bool removeAfterUpload = false)
        {
            bool result = false;
            foreach (var avatar in uploadingAvatars)
            {
                bool success = UploadAvatar(avatar);
                result = result || success;
                if (removeAfterUpload)
                {
                    FileUtils.RemoveFile(avatar.AvatarFilePath);
                    FileUtils.RemoveFile(avatar.AvatarFilePath + ".manifest");
                }
            }

            return result;
        }

        private bool UploadAvatar(UploadAvatarInfo avatar)
        {
            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(avatar.DisplayName), "displayName");
            formData.Add(new StringContent(avatar.AvatarGender.ToString()), "gender");

            byte[] imgFileData = DataConverter.SpriteToRowData(avatar.Image);
            var imgFileContent = new ByteArrayContent(imgFileData);
            imgFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            formData.Add(imgFileContent, "img", avatar.Image.name);

            var avatarFileStream = new FileStream(avatar.AvatarFilePath, FileMode.Open, FileAccess.Read);
            var avatarFileContent = new StreamContent(avatarFileStream);
            avatarFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            formData.Add(avatarFileContent, "avatar", avatar.Name);
            
            try
            {
                ResponseDetails response = restAPI.PostMultipartRequest<ResponseDetails>(UPLOAD_AVATAR_URL, formData);
                if (!response.success)
                {
                    AppLogger.Warning($"Upload Avatar request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Upload Avatar request ended with error: {ex.Message}");
            }
            finally
            {
                // Закрываем потоки после завершения запроса
                avatarFileStream.Dispose();
            }
            
            return true;
        }

        /// <summary>
        /// <para>Удаляет множество аватаров.</para>
        /// </summary>
        /// <param name="deletedNames">список удаляемых аватаров</param>
        /// <returns>true, если удаление прошло успешно</returns>
        public bool DeleteManyAvatars(List<string> deletedNames)
        {
            DeleteByNamesRequest request = new DeleteByNamesRequest()
            {
                names = deletedNames
            };


            ResponseDetails response =
                restAPI.PostRequest<DeleteByNamesRequest, ResponseDetails>(DELETE_AVATARS_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Delete Avatars request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <para>Получает файлы указанных аватаров с файлового сервера.</para>
        /// </summary>
        /// <param name="downloadAvatarsInfos">информация об аватарах, файлы которых необходимо загрузить</param>
        /// <returns>информация об успешно загруженных файлах</returns>
        public async Task<IList<AvatarInfo>> GetAvatarsFiles(IList<AvatarInfo> downloadAvatarsInfos)
        {
            IList<AvatarInfo> result = new List<AvatarInfo>(downloadAvatarsInfos);
            for (int i = result.Count - 1; i >= 0; i--)
            {
                AvatarInfo avatar = downloadAvatarsInfos[i];
                AppLogger.Log($"Downloading avatar {avatar.Name}");
                SaveFileRequest saveFileRequest = SaveFileRequest.Form(AvatarAssetPackages.ASSETS_DIRECTORY, avatar.Name);
                bool success = await restAPI.GetFileRequest($"{AVATAR_FILE_URL}/{avatar.Name}", saveFileRequest);
                if (!success)
                {
                    result.RemoveAt(i);
                }
            }

            return result;
        }
    }
}