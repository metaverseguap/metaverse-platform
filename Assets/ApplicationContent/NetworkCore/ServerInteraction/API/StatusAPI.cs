using System.Threading.Tasks;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.API
{
    /// <summary>
    /// <para>Взаимодействие с /api/status файлового сервера.</para>
    /// </summary>
    public sealed class StatusAPI : AbstractServerAPI
    {
        private const string STATUS_URL = "/api/status";
        
        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="serverUri">uri файлового сервера</param>
        public StatusAPI(string serverUri) : base(serverUri)
        {
        }

        /// <summary>
        /// <para>Проверяет доступен ли файловый сервер.</para>
        /// </summary>
        /// <returns>true, если файловый сервер доступен</returns>
        public async Task<bool> IsServerOnline()
        {
            ResponseDetails result = await restAPI.ExecuteAsyncRequest(
                (token) => restAPI.AsyncGetRequest<ResponseDetails>(STATUS_URL, token, false)
            );

            return result.success;
        }
    }
}