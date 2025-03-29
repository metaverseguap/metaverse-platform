using UnityEngine;

namespace Player.EmbeddedPlayers
{
    /// <summary>
    /// <para>Базовый класс контроллера игрока.</para>
    ///
    /// Все контроллеры игрока должны быть наследниками данного класса
    /// </summary>
    public abstract class AbstractPlayerController : MonoBehaviour
    {
        /// <summary>
        /// <para>Является ли данный контроллер активным.</para>
        ///
        /// <remarks>данное поле необходимо, когда в одной сцене находиться несколько контроллеров игрока.
        /// В этом случае нужно фиксировать пользовательский ввод только на активном контроллере.</remarks>
        /// </summary>
        protected bool isActiveController = true;
        
        /// <summary>
        /// <para>Является ли данный контроллер активным.</para>
        ///
        /// <remarks>данное свойство необходимо, когда в одной сцене находиться несколько контроллеров игрока.
        /// В этом случае нужно фиксировать пользовательский ввод только на активном контроллере.</remarks>
        /// </summary>
        public bool ActiveController
        {
            get => isActiveController;
            set => isActiveController = value;
        }

        /// <summary>
        /// <para>Деактивировать контроллер навсегда.</para>
        /// 
        /// <remarks>данный метод необходим, когда мы хотим деактивировать контроллер и все его управляющие части навсегда</remarks>
        /// </summary>
        public virtual void DeactivatePermanently()
        {
            isActiveController = false;
        }
    }
}