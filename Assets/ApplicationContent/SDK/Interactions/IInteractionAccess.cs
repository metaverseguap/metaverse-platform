using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Interactions
{
    /// <summary>
    /// <para>Интерфейс компонента, проверяющего возможность взаимодействия с интерактивным объектом в данный момент.</para>
    /// </summary>
    public interface IInteractionAccess
    {
        /// <summary>
        /// <para>Разрешена ли интерактивность с объектом в данный момент.</para>
        /// </summary>
        /// <returns>true, если в данный момент можно взаимодействовать с объектом</returns>
        bool IsInteractionAllowed();

        /// <summary>
        /// <para>Контролирует ли указанный контроллер интерактивность в данный момент.</para>
        /// </summary>
        /// <param name="controller">объект контроллер</param>
        /// <returns>true, если указанный контроллер в данный момент контролирует интерактивность</returns>
        bool HasInteractionControl(GameObject controller);

        /// <summary>
        /// <para>Метод вызываемый при получении контроля над интерактивностью объекта.</para>
        /// </summary>
        /// <param name="controllingObject">объект контроллера, получивший контроль над интерактивностью объекта</param>
        void AcquireControl(GameObject controllingObject);

        /// <summary>
        /// <para>Метод вызываемый при потере контроля над интерактивностью объекта.</para>
        /// </summary>
        void ReleaseControl();
    }
}