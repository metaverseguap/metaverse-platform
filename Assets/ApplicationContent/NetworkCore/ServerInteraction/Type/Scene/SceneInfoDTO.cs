namespace NetworkCore.ServerInteraction.Type.Scene
{
    /// <summary>
    /// <para>Объект передачи данных информации о сцене.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SceneInfoDTO
    {
        /// <summary>
        /// Индекс сортировки сцены.
        /// </summary>
        public int sortIndex { get; set; }

        /// <summary>
        /// Имя файла сцены.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Отображаемое имя сцены.
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// Устройство, для которого сделана сцена.
        /// </summary>
        public string device { get; set; }

        /// <summary>
        /// Путь до файла изображения сцены на сервере.
        /// </summary>
        public string imageFilePath { get; set; }

        /// <summary>
        /// Изображение сцены.
        /// </summary>
        public byte[] imageData { get; set; }
    }
}