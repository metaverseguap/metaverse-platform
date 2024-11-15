using UnityEngine;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер для информации об аватаре.</para>
    /// </summary>
    public class AvatarInfo
    {
        /// <summary>
        /// Имя файла аватара.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отображаемое имя аватара.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Изображение аватара.
        /// </summary>
        public Sprite Image { get; set; }
    }
}