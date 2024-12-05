using System.Collections.Generic;
using Global.Logger;
using MainMenu.Containers;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.LoginKey;
using NetworkCore.ServerInteraction.Type.LoginKey.Request;
using NetworkCore.ServerInteraction.Type.LoginKey.Response;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с /api/login-key файлового сервера.</para>
    /// </summary>
    public sealed class LoginKeyAPI : AbstractServerAPI
    {
        private const string ALL_KEYS_URL = "/api/login-key/all";
        private const string CREATE_KEY_URL = "/api/login-key/create";
        private const string DELETE_KEYS_URL = "/api/login-key/delete-by-names";

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public LoginKeyAPI(string serverUri) : base(serverUri)
        {
        }

        /// <summary>
        /// <para>Получает все ключи авторизации.</para>
        /// </summary>
        /// <returns>список всех ключей авторизации</returns>
        public IList<LoginKeyInfo> GetAllLoginKeys()
        {
            LoginKeysResponse response = restAPI.GetRequest<LoginKeysResponse>(ALL_KEYS_URL);
            if (response.success)
            {
                IList<LoginKeyInfo> result = new List<LoginKeyInfo>();

                foreach (var loginKeyRO in response.keys)
                {
                    result.Add(
                        new LoginKeyInfo()
                        {
                            Key = loginKeyRO.key,
                            DateTo = loginKeyRO.dateTo,
                            DateFrom = loginKeyRO.dateFrom
                        }
                    );
                }

                return result;
            }
            else
            {
                AppLogger.Warning($"Get All Login keys request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return new List<LoginKeyInfo>();
        }

        /// <summary>
        /// <para>Создает ключ авторизации на файловом сервере.</para>
        /// </summary>
        /// <param name="loginKeyInfo"><see cref="LoginKeyInfo">ключ авторизации</see></param>
        /// <param name="exceptionMessage">сообщение об ошибках создания ключа</param>
        /// <returns>true, если ключь был успешно создан</returns>
        public bool CreateLoginKey(LoginKeyInfo loginKeyInfo, out string exceptionMessage)
        {
            CreateLoginKeyRequest request = new CreateLoginKeyRequest();
            request.loginKey = new LoginKeyDTO()
            {
                key = loginKeyInfo.Key,
                dateFrom = loginKeyInfo.DateFrom,
                dateTo = loginKeyInfo.DateTo
            };

            ResponseDetails response =
                restAPI.PostRequest<CreateLoginKeyRequest, ResponseDetails>(CREATE_KEY_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Create Login Key request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                exceptionMessage = ResponseUtils.GetErrorMessagesAsString(response);
                return false;
            }

            exceptionMessage = "";
            return true;
        }

        /// <summary>
        /// <para>Удаляет множество ключей авторизации.</para>
        /// </summary>
        /// <param name="deletedNames">список удаляемых ключей авторизации</param>
        /// <returns>true, если удаление прошло успешно</returns>
        public bool DeleteMany(List<string> deletedNames)
        {
            DeleteByNamesRequest request = new DeleteByNamesRequest()
            {
                names = deletedNames
            };


            ResponseDetails response = restAPI.PostRequest<DeleteByNamesRequest, ResponseDetails>(DELETE_KEYS_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Delete Login Keys request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }
    }
}