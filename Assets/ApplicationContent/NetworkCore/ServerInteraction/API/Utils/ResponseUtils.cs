using System.Text;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.API.Utils
{
    /// <summary>
    /// <para>Набор вспомогательных методов для обработки результатов запросов.</para>
    /// </summary>
    public static class ResponseUtils
    {
        /// <summary>
        /// <para>Получить из <see cref="ResponseDetails"/> сообщение об ошибке.</para>
        /// </summary>
        /// <param name="details"><see cref="ResponseDetails"/></param>
        /// <returns>сообщение об ошибке</returns>
        public static string GetErrorMessagesAsString(ResponseDetails details)
        {
            StringBuilder message = new StringBuilder();
            foreach (var error in details.error)
            {
                message.Append($"[Code {error.code}]: {error.exceptionMessage}\n");
            }
            message.Length = message.Length - 1;
            
            return message.ToString();
        }
    }
}