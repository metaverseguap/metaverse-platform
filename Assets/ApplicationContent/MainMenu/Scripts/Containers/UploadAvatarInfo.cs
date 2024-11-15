namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер хранящий данный загружаемого на сервер аватара.</para>
    /// </summary>
    public sealed class UploadAvatarInfo : AvatarInfo
    {
        /// <summary>
        /// Путь до файла аватара.
        /// </summary>
        public string AvatarFilePath { get; set; }
    }
}