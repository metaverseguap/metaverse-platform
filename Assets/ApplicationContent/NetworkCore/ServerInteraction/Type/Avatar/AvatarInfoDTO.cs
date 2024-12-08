namespace NetworkCore.ServerInteraction.Type.Avatar
{
    /// <summary>
    /// <para>Объект передачи данных информации об аватаре.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class AvatarInfoDTO
    {
        /// <summary>
        /// Имя файла аватара.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Отображаемое имя аватара.
        /// </summary>
        public string displayName { get; set; }
        
        /// <summary>
        /// Пол аватара.
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// Путь до файла изображения аватара на сервере.
        /// </summary>
        public string imageFilePath { get; set; }

        /// <summary>
        /// Изображение аватара.
        /// </summary>
        public byte[] imageData { get; set; }
    }
}