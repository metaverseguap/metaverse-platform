using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Scene.Response
{
    /// <summary>
    /// <para>Объект результата запроса множества информаций о датах обновления файлов сцен.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SceneUpdateInfosResponse : ResponseDetails
    {
        /// <summary>
        /// Список с информацией о датах обновления файлов сцен.
        /// </summary>
        public List<SceneUpdateInfoDTO> updateInfos { get; set; }
    }
}