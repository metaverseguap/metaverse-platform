using UnityEngine;

namespace Global.Controllers.Cursor
{
    /// <summary>
    /// <para>Скрипт, управляющий статусом курсора.</para>
    /// </summary>
    public sealed class CursorHandler : MonoBehaviour
    {
        [SerializeField] private bool _cursorEnabled = true;
        [SerializeField] private bool _force = true;

        private void Start()
        {
            if (_cursorEnabled)
                if (_force)
                    CursorController.ShowCursorForceSingle();
                else
                    CursorController.ShowCursor();
            else if (_force)
                CursorController.HideCursorForce();
            else
                CursorController.HideCursor();
        }
    }
}