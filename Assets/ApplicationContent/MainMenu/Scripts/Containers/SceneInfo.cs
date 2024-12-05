using MainMenu.Containers.Interfaces;
using NetworkCore.MirrorNetworking.Types.Devices;
using UnityEngine;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер для информации о сцене.</para>
    /// </summary>
    public class SceneInfo : INamedContainer, IContainerWithImage
    {
        /// <summary>
        /// Имя файла сцены.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отображаемое имя сцены.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Устройство, для которого сделана сцена.
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// Изображение сцены.
        /// </summary>
        public Sprite Image { get; set; }
        
        /// <summary>
        /// Индекс сортировки сцены.
        /// </summary>
        public int SortIndex { get; set; }
    }
}