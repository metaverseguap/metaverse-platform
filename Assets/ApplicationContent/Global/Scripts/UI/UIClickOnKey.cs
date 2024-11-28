using UnityEngine;
using UnityEngine.UI;

namespace Global.UI
{
    /// <summary>
    /// <para>Класс активирующий кнопку по нажатию клавиши.</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class UIClickOnKey : MonoBehaviour
    {
        [SerializeField] private KeyCode _key = KeyCode.Return;
        
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key))
            {
                button.onClick.Invoke();
            }
        }
    }
}