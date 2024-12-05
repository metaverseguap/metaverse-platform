using UnityEngine;

namespace MainMenu.Containers.Interfaces
{
    /// <summary>
    /// <para>Контейнер имеющий поле Image.</para>
    /// </summary>
    public interface IContainerWithImage
    {
        /// <summary>
        /// Изображение.
        /// </summary>
        public Sprite Image { get; set; }
    }
}