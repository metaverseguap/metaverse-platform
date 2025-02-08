namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Контейнер отвечающий за хранение классов взаимодействия с файловым сервером.</para>
    /// </summary>
    public sealed class APIContainer
    {
        /// <summary>
        /// <inheritdoc cref="AuthAPI"/>
        /// </summary>
        public AuthAPI Auth { get; private set; }

        /// <summary>
        /// <inheritdoc cref="RoleAPI"/>
        /// </summary>
        public RoleAPI Role { get; private set; }

        /// <summary>
        /// <inheritdoc cref="LoginKeyAPI"/>
        /// </summary>
        public LoginKeyAPI LoginKey { get; private set; }

        /// <summary>
        /// <inheritdoc cref="RegistrationKeyAPI"/>
        /// </summary>
        public RegistrationKeyAPI RegistrationKey { get; private set; }

        /// <summary>
        /// <inheritdoc cref="SceneAPI"/>
        /// </summary>
        public SceneAPI Scene { get; private set; }
        
        /// <summary>
        /// <inheritdoc cref="AvatarAPI"/>
        /// </summary>
        public AvatarAPI Avatar { get; private set; }
        
        /// <summary>
        /// <inheritdoc cref="UserAPI"/>
        /// </summary>
        public UserAPI User { get; private set; }
        
        /// <summary>
        /// <inheritdoc cref="HostsAPI"/>
        /// </summary>
        public HostsAPI Hosts { get; private set; }

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public APIContainer(string serverUri)
        {
            this.Auth = new AuthAPI(serverUri, this);
            this.Role = new RoleAPI(serverUri);
            this.LoginKey = new LoginKeyAPI(serverUri);
            this.RegistrationKey = new RegistrationKeyAPI(serverUri);
            this.Scene = new SceneAPI(serverUri);
            this.Avatar = new AvatarAPI(serverUri);
            this.User = new UserAPI(serverUri);
            this.Hosts = new HostsAPI(serverUri);
        }

        /// <summary>
        /// <para>Устанавливает jwt всем хранимым api.</para>
        /// </summary>
        /// <param name="token">JSON Web Token</param>
        public void SetAuthToken(string token)
        {
            this.Auth.SetAuthorization(token);
            this.Role.SetAuthorization(token);
            this.LoginKey.SetAuthorization(token);
            this.RegistrationKey.SetAuthorization(token);
            this.Scene.SetAuthorization(token);
            this.Avatar.SetAuthorization(token);
            this.User.SetAuthorization(token);
            this.Hosts.SetAuthorization(token);
        }
    }
}