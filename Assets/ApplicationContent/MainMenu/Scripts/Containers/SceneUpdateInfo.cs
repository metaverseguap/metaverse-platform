using System;
using Global.Containers;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер для информации о дате обновления сцены на файловом сервере.</para>
    /// </summary>
    public sealed class SceneUpdateInfo : INamedContainer
    {
        /// <summary>
        /// Имя файла сцены.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Дата обновления файла.
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}