using UnityEngine;

namespace Global.Controllers.Cursor
{
    /// <summary>
    /// <para>Контроллер курсора.</para>
    /// </summary>
    public sealed class CursorController : MonoBehaviour
    {
        // Увеличивается на 1 когда где-то нужен курсор. Уменьшается когда больше не нужен. Курсор видим когда > 0
        private static int activeCursorCounter = 0;

        /// <summary>
        /// <para>Показать курсор.</para>
        /// </summary>
        public static void ShowCursor()
        {
            activeCursorCounter++;
            UpdateCursor();
        }

        /// <summary>
        /// <para>Показать курсор в любом случае.</para>
        /// </summary>
        public static void ShowCursorForceSingle()
        {
            activeCursorCounter = 1;
            UpdateCursor();
        }

        /// <summary>
        /// <para>Скрыть курсор.</para>
        /// </summary>
        public static void HideCursor()
        {
            if (activeCursorCounter > 0)
                activeCursorCounter--;
            UpdateCursor();
        }

        /// <summary>
        /// <para>Скрыть курсор в любом случае.</para>
        /// </summary>
        public static void HideCursorForce()
        {
            activeCursorCounter = 0;
            UpdateCursor();
        }

        private static void UpdateCursor()
        {
            if (activeCursorCounter > 0)
            {
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}