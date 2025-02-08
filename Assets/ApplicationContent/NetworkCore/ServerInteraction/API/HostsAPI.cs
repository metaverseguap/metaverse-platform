using System.Collections.Generic;
using Global.Logger;
using MainMenu.Containers;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.Host.Request;
using NetworkCore.ServerInteraction.Type.Host.Response;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с /api/hosts файлового сервера.</para>
    /// </summary>
    public sealed class HostsAPI : AbstractServerAPI
    {
        private const string HOSTS_URL = "/api/hosts";
        private const string CREATE_HOSTS_URL = "/api/hosts/create";
        private const string DELETE_HOSTS_URL = "/api/hosts/delete";

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public HostsAPI(string serverUri) : base(serverUri)
        {
        }

        /// <summary>
        /// <para>Получает информацию о хостах указанной сцены.</para>
        /// </summary>
        /// <param name="sceneName">имя сцены</param>
        /// <returns>информация о всех хостах указанной сцены</returns>
        public IList<HostInfo> GetHostsBySceneName(string sceneName)
        {
            IList<HostInfo> result = new List<HostInfo>();
            GetParam sceneNameParam = GetParam.Form("sceneName", sceneName);

            SinglesceneHostsResponse response = restAPI.GetRequest<SinglesceneHostsResponse>(HOSTS_URL, sceneNameParam);

            if (response.success)
            {
                foreach (var infoRO in response.hosts)
                {
                    HostInfo info = new HostInfo();
                    info.Login = infoRO.login;
                    info.DisplayName = infoRO.name;
                    info.Uri = infoRO.uri;
                    info.SceneName = infoRO.sceneName;

                    result.Add(info);
                }

                return result;
            }
            else
            {
                AppLogger.Error($"Host info request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return result;
        }

        /// <summary>
        /// <para>Сообщает серверу, что текущий пользователь становится хостом для указанной сцены.</para>
        /// </summary>
        /// <param name="url">url подключения к хосту</param>
        /// <param name="sceneName">имя сцены, хостом которой стал пользователь</param>
        /// <returns>true, если запрос завершился успешно</returns>
        public bool BecomeAHost(string url, string sceneName)
        {
            CreateHostRequest request = new CreateHostRequest();
            request.uri = url;
            request.sceneName = sceneName;

            ResponseDetails response = restAPI.PostRequest<CreateHostRequest, ResponseDetails>(CREATE_HOSTS_URL, request);

            if (!response.success)
            {
                AppLogger.Warning($"Create Host request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// <para>Запрос на удаление хоста текущего пользователя на файловом сервере.</para>
        /// </summary>
        /// <returns>true, если запрос завершился успешно</returns>
        public bool RemoveHost()
        {
            ResponseDetails response = restAPI.DeleteRequest<ResponseDetails>(DELETE_HOSTS_URL);
            if (response.success)
            {
                return true;
            }
            else
            {
                AppLogger.Warning($"Delete host request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
                return false;
            }
        }
    }
}