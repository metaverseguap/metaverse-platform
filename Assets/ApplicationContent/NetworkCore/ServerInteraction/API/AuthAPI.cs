using MainMenu.Containers;
using NetworkCore.MirrorNetworking;
using NetworkCore.ServerInteraction.API.Utils;
using NetworkCore.ServerInteraction.Type.Auth.Request;
using NetworkCore.ServerInteraction.Type.Auth.Response;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с api/auth файлового сервера.</para>
    /// </summary>
    public sealed class AuthAPI : AbstractServerAPI
    {
        private const string LOGIN_URL = "/api/auth/login";
        private const string REGISTRATION_URL = "/api/auth/registration";

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public AuthAPI(string serverUri) : base(serverUri)
        {
        }

        /// <summary>
        /// <para>Осуществляет авторизацию на файловом сервере.</para>
        /// </summary>
        /// <param name="login">Login пользователя</param>
        /// <param name="password">пароль пользователя</param>
        /// <param name="authCode">код авторизации</param>
        /// <param name="exceptionMessage">сообщение об ошибках авторизации</param>
        /// <returns>true, если запрос завершен успешно</returns>
        public bool Login(string login, string password, string authCode, out string exceptionMessage)
        {
            LoginRequest request = new LoginRequest();
            request.login = login;
            request.password = password;
            request.loginKey = authCode;
            
            AuthResponse response = restAPI.PostRequest<LoginRequest, AuthResponse>(LOGIN_URL, request);

            if (!response.success)
            {
                exceptionMessage = ResponseUtils.GetErrorMessagesAsString(response);
                return false;
            }
            
            restAPI.SetAuthorization($"Bearer {response.token}");
            MVNetworkManager.singleton.NetworkStore.MyPlayerInfo.Login = login;

            exceptionMessage = "";
            return true;
        }

        /// <summary>
        /// <para>Осуществляет регистрацию на файловом сервере.</para>
        /// </summary>
        /// <param name="registrationInfo">регистрационные данные пользователя</param>
        /// <param name="exceptionMessage">сообщение об ошибках авторизации</param>
        /// <returns>true, если запрос завершен успешно</returns>
        public bool Registration(RegistrationInfo registrationInfo, out string exceptionMessage)
        {
            RegistrationRequest request = new RegistrationRequest();
            request.login = registrationInfo.Login;
            request.name = registrationInfo.NickName;
            request.password = registrationInfo.Password;
            request.repeatPassword = registrationInfo.RepeatPassword;
            request.registrationKey = registrationInfo.RegistrationKey;
            
            AuthResponse response = restAPI.PostRequest<RegistrationRequest, AuthResponse>(REGISTRATION_URL, request);

            if (!response.success)
            {
                exceptionMessage = ResponseUtils.GetErrorMessagesAsString(response);
                return false;
            }
            
            restAPI.SetAuthorization($"Bearer {response.token}");
            
            exceptionMessage = "";
            return true;
        }
    }
}