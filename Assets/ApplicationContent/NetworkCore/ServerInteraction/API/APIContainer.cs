using System.Collections.Generic;
using NetworkCore.Utils;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Контейнер отвечающий за хранение классов взаимодействия с файловым сервером.</para>
    /// </summary>
    public sealed class APIContainer
    {
        
        private string serverAddress;
        
        /// <summary>
        /// <para>Адрес файлового сервера.</para>
        /// Данный <see cref="APIContainer"/> использует указанный адрес для подключения к файловому серверу
        /// </summary>
        public string ServerAddress
        {
            get => serverAddress;
            set
            {
                if (IPUtils.IsURLValid(value))
                {
                    serverAddress = value;
                    ChangeServerAddress(serverAddress);
                }
            }
        }
        
        /// <summary>
        /// <inheritdoc cref="AuthAPI"/>
        /// </summary>
        public AuthAPI Auth { get; }

        /// <summary>
        /// <inheritdoc cref="RoleAPI"/>
        /// </summary>
        public RoleAPI Role { get; }

        /// <summary>
        /// <inheritdoc cref="LoginKeyAPI"/>
        /// </summary>
        public LoginKeyAPI LoginKey { get; }

        /// <summary>
        /// <inheritdoc cref="RegistrationKeyAPI"/>
        /// </summary>
        public RegistrationKeyAPI RegistrationKey { get; }

        /// <summary>
        /// <inheritdoc cref="SceneAPI"/>
        /// </summary>
        public SceneAPI Scene { get; }
        
        /// <summary>
        /// <inheritdoc cref="AvatarAPI"/>
        /// </summary>
        public AvatarAPI Avatar { get; }
        
        /// <summary>
        /// <inheritdoc cref="UserAPI"/>
        /// </summary>
        public UserAPI User { get; }
        
        /// <summary>
        /// <inheritdoc cref="HostsAPI"/>
        /// </summary>
        public HostsAPI Hosts { get; }
        
        /// <summary>
        /// <inheritdoc cref="StatusAPI"/>
        /// </summary>
        public StatusAPI ServerStatus { get; }
        
        private IList<AbstractServerAPI> apiList;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public APIContainer(string serverUri)
        {
            serverAddress = serverUri;
            
            this.Auth = new AuthAPI(serverUri, this);
            this.Role = new RoleAPI(serverUri);
            this.LoginKey = new LoginKeyAPI(serverUri);
            this.RegistrationKey = new RegistrationKeyAPI(serverUri);
            this.Scene = new SceneAPI(serverUri);
            this.Avatar = new AvatarAPI(serverUri);
            this.User = new UserAPI(serverUri);
            this.Hosts = new HostsAPI(serverUri);
            this.ServerStatus = new StatusAPI(serverUri);
            
            apiList = new List<AbstractServerAPI>()
            {
                Auth, Role, LoginKey, RegistrationKey,
                Scene, Avatar, User, Hosts,
                ServerStatus
            };
        }

        private void ChangeServerAddress(string newServerAddress)
        {
            foreach (var api in apiList)
            {
                api.SetServerUrl(newServerAddress);
            }
        }

        /// <summary>
        /// <para>Устанавливает jwt всем хранимым api.</para>
        /// </summary>
        /// <param name="token">JSON Web Token</param>
        public void SetAuthToken(string token)
        {
            foreach (var api in apiList)
            {
                api.SetAuthorization(token);
            }
        }
    }
}