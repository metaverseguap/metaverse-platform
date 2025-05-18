using NetworkCore.ServerInteraction.API.Utils;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Базовые поля и методы для взаимодействия с файловым сервером.</para>
    /// </summary>
    public abstract class AbstractServerAPI
    {
        /// <summary>
        /// <in cref="RestAPI"/>
        /// </summary>
        protected readonly RestAPI restAPI;
        
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        protected AbstractServerAPI(string serverUri)
        {
            this.restAPI = RestAPI.singleton(serverUri);
        }
        
        /// <summary>
        /// <inheritdoc cref="RestAPI.SetAuthorization"/>
        /// </summary>
        public void SetAuthorization(string token)
        {
            restAPI.SetAuthorization(token);
        }
        
        /// <summary>
        /// <para>Задать url сервера.</para>
        /// </summary>
        /// <param name="serverUrl">основной url сервера. Он будет подставляться в начало конкретных запросов</param>
        public void SetServerUrl(string serverUrl)
        {
            restAPI.SetBaseUrl(serverUrl);
        }
    }
}