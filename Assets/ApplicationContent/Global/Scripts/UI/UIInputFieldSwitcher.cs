using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Global.UI
{
    /// <summary>
    /// <para>Данный компонент переключает фокус между полями ввода на UI по нажатию клавиши.</para>
    /// </summary>
    public sealed class UIInputFieldSwitcher : MonoBehaviour
    {
        [SerializeField] private List<TMP_InputField> _inputFields;
        [SerializeField] private KeyCode _key = KeyCode.Tab;

        private int selectedIndex = 0;

        private void Update()
        {
            if (_inputFields.Count > 0 && _inputFields.Exists(field => field.isFocused) && Input.GetKeyDown(_key))
            {
                selectedIndex = (_inputFields.FindIndex(field => field.isFocused) + 1) % _inputFields.Count;
                _inputFields[selectedIndex].ActivateInputField();
            }
        }
    }
}