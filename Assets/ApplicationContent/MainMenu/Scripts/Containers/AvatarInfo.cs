using System;
using AppAvatars.Types;
using Global.Containers;
using UnityEngine;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер для информации об аватаре.</para>
    /// </summary>
    public class AvatarInfo : INamedContainer, IContainerWithImage
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
        /// Тип контроллера анимации.
        /// </summary>
        public AnimationControllerType AvatarAnimationControllerType { get; set; }

        /// <summary>
        /// Изображение аватара.
        /// </summary>
        public Sprite Image { get; set; }
        
        /// <summary>
        /// Дата обновления файла.
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}