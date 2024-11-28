using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response.Details;

namespace NetworkCore.ServerInteraction.Type.Response
{
    /// <summary>
    /// <para>Объект результата выполнения запроса.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public class ResponseDetails
    {
        /// <summary>
        /// Была ли запрос успешно выполнена.
        /// </summary>
        public bool success { get; set; } = false;

        /// <summary>
        /// Список информирующих сообщений.
        /// </summary>
        public List<InfoDetails> info { get; set; }

        /// <summary>
        /// Список предупреждений.
        /// </summary>
        public List<InfoDetails> warning { get; set; }

        /// <summary>
        /// Список ошибок, в случае если запрос не был выполнен успешно.
        /// </summary>
        public List<ErrorDetails> error { get; set; }
    }
}