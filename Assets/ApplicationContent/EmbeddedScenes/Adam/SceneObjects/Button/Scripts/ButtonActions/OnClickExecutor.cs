using UnityEngine;
using UnityEngine.Events;

namespace Adam.SceneObjects.Button.ButtonActions
{
    /// <summary>
    /// <para>Компоненты вызывающий метод по нажатию на кнопку.</para>
    /// </summary>
    public sealed class OnClickExecutor : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent _onPressed;
        [Tooltip("Разрешить взаимодействие через зажатие клавиши")]
        [SerializeField] private bool _holdInteraction;

        public bool AllowHoldInteraction => _holdInteraction;
        
        public void Interact()
        {
            _onPressed?.Invoke();
        }
    }
}