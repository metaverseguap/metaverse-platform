using System.Threading.Tasks;
using Global.Logger;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.Request;
using NetworkCore.ServerInteraction.Type.Response;
using NetworkCore.ServerInteraction.Type.User.Response;
using UserSystem.Types;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с /api/user файлового сервера.</para>
    /// </summary>
    public sealed class UserAPI : AbstractServerAPI
    {

        private const string USER_BY_LOGIN_URL = "/api/user/user-info-by-login";
        private const string MY_USER_URL = "/api/user/me";
        private const string UPDATE_USER_STATUS_URL = "/api/user/update-status";
        
        private readonly int statusUpdateInterval;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        /// <param name="statusUpdateInterval">интервал обновления статуса пользователя на сервере</param>
        public UserAPI(string serverUri, int statusUpdateInterval) : base(serverUri)
        {
            this.statusUpdateInterval = statusUpdateInterval;
        }

        /// <summary>
        /// Интервал обновления статуса пользователя на сервере (в секундах).
        /// </summary>
        public int StatusUpdateInterval => statusUpdateInterval;

        /// <summary>
        /// <para>Получает пользователя по его login.</para>
        /// </summary>
        /// <param name="login">login пользователя</param>
        /// <returns><see cref="UserInfo">информация о пользователе</see></returns>
        public UserInfo GetUserByLogin(string login)
        {
            GetParam loginParam = GetParam.Form("login", login);

            UserInfoResponse response = restAPI.GetRequest<UserInfoResponse>(USER_BY_LOGIN_URL, loginParam);
            
            if (response.success)
            {
                UserInfo result = new UserInfo();
                result.Login = response.userInfo.login;
                result.Nickname = response.userInfo.name;
                result.Role = RoleAPI.ConvertToRoleInfo(response.userInfo.role);

                return result;
            }
            else
            {
                AppLogger.Warning($"Get User request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return new UserInfo();
        }
        
        /// <summary>
        /// <para>Получает информацию о текущем пользователе.</para>
        /// </summary>
        /// <returns><see cref="UserInfo">информация о пользователе</see></returns>
        public UserInfo GetMyUser()
        {
            UserInfoResponse response = restAPI.GetRequest<UserInfoResponse>(MY_USER_URL);
            if (response.success)
            {
                UserInfo result = new UserInfo();
                result.Login = response.userInfo.login;
                result.Nickname = response.userInfo.name;
                result.Role = RoleAPI.ConvertToRoleInfo(response.userInfo.role);

                return result;
            }
            else
            {
                AppLogger.Warning($"Get User request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }

            return new UserInfo();
        }

        /// <summary>
        /// <para>Обновить свой статус на сервере.</para>
        /// </summary>
        public async Task UpdateMyServerStatus()
        {
            ResponseDetails response = await restAPI.ExecuteAsyncRequest(
                (token) => restAPI.AsyncGetRequest<ResponseDetails>(UPDATE_USER_STATUS_URL, token, false)
            );

            if (response.success)
            {
                return;
            }
            else
            {
                AppLogger.Warning($"Update User status request ended with error: {ResponseUtils.GetErrorMessagesAsString(response)}");
            }
        }
    }
}