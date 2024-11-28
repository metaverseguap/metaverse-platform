using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.UI
{
    /// <summary>
    /// <para>Скрипт, отключающий кнопку вне Unity Editor.</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class UIEditorButton : MonoBehaviour
    {
        private Button button;
        
        private void OnEnable()
        {
            EnsureButton();
            button.interactable = false;
            ActivateButton();
        }

        private void EnsureButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void ActivateButton()
        {
            button.interactable = true;
        }
    }
}