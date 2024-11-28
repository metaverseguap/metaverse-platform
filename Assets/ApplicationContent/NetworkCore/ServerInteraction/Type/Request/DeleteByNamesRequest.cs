using System.Collections.Generic;

namespace NetworkCore.ServerInteraction.Type.Request
{
    /// <summary>
    /// <para>Объект запроса удаления нескольких объектов по их именам.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class DeleteByNamesRequest
    {
        /// <summary>
        /// Имена удаляемых объектов.
        /// </summary>
        public List<string> names { get; set; }
    }
}