using MainMenu.Containers.Interfaces;
using NetworkCore.MirrorNetworking.Types.Devices;
using UnityEngine;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер для информации о сцене.</para>
    /// </summary>
    public class SceneInfo : INamedContainer, IContainerWithImage, ICloneableContainer<SceneInfo>
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
        /// <para>Путь до файла сцены в приложении.</para>
        /// Данное свойство нужно при загрузке ассетов сцен
        /// </summary>
        public string CachedPath { get; set; }
        
        /// <summary>
        /// <para>Конвейерный метод задания поля <see cref="CachedPath"/></para>
        /// </summary>
        /// <param name="path">путь до файла сцены в приложении</param>
        /// <returns>self</returns>
        public SceneInfo WithCachedPath(string path)
        {
            this.CachedPath = path;
            return this;  
        }

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

        /// <summary>
        /// <inheritdoc cref="ICloneableContainer.Clone"/>
        /// </summary>
        /// <returns><inheritdoc cref="ICloneableContainer.Clone"/></returns>
        public SceneInfo Clone()
        {
            SceneInfo clone = new SceneInfo();
            clone.Name = Name;
            clone.DisplayName = DisplayName;
            clone.CachedPath = CachedPath;
            clone.Device = Device;
            clone.Image = Image;
            clone.SortIndex = SortIndex;

            return clone;
        }

        /// <summary>
        /// <para>Получить имя по которому будет производиться загрузка сцены.</para>
        /// Если сцена добавлена в Build приложения, то она загружается по имени сцены.
        /// Однако, если сцена загружалась через Asset Bundle,
        /// то такая сцена должна загружаться по пути до сцены в приложении
        /// </summary>
        /// <returns>имя сцены по которому будет производиться загрузка сцены</returns>
        public string GetSceneLoadingName()
        {
            return CachedPath != null ? CachedPath : Name;
        }
    }
}