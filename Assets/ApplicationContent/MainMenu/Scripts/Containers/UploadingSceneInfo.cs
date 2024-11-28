namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер хранящий данный загружаемой на сервер сцены.</para>
    /// </summary>
    public sealed class UploadingSceneInfo : SceneInfo
    {
        /// <summary>
        /// Путь до файла сцены.
        /// </summary>
        public string SceneFilePath { get; set; }
    }
}