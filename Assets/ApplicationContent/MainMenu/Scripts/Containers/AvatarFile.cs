using AppAvatars.Types;
using MainMenu.Containers.Interfaces;
using UnityEngine;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер хранящий файл аватара.</para>
    /// </summary>
    public sealed class AvatarFile : INamedContainer, IContainerWithImage
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
        /// Пол аватара.
        /// </summary>
        public Gender AvatarGender { get; set; }

        /// <summary>
        /// Изображение аватара.
        /// </summary>
        public Sprite Image { get; set; }

        /// <summary>
        /// Модель аватара.
        /// </summary>
        public GameObject Model { get; set; }
    }
}