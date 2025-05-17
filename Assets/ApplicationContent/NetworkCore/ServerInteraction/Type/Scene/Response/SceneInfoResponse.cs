using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Scene.Response
{
    /// <summary>
    /// <para>Объект результата запроса информации о сцене.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SceneInfoResponse : ResponseDetails
    {
        /// <summary>
        /// Информация о сцене.
        /// </summary>
        public SceneInfoDTO sceneInfo { get; set; }
    }
}