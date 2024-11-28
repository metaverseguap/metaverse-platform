using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Scene.Response
{
    /// <summary>
    /// <para>Объект результата запроса множества информаций о сценах.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SceneInfosResponse : ResponseDetails
    {
        /// <summary>
        /// Список с информацией о сценах.
        /// </summary>
        public List<SceneInfoRO> infoList { get; set; }
    }
}