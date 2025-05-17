using System;

namespace NetworkCore.ServerInteraction.Type.Scene
{
    /// <summary>
    /// <para>Объект передачи данных информации о дате обновления сцены.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class SceneUpdateInfoDTO
    {
        /// <summary>
        /// Имя файла сцены.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Дата обновления файла.
        /// </summary>
        public DateTime updateDate { get; set; }
    }
}