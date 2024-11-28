using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Global.UI.ScrollList
{

    /// <summary>
    /// <para>Класс меняет цвет объекта, когда checkbox меняет свое состояние.</para>
    /// </summary>
    public sealed class UIChangeColorOnSelected : MonoBehaviour
    {
        [SerializeField] private Toggle _checkbox;
        [SerializeField] private List<Graphic> _changeColorObjects;
        [SerializeField] private Color _onColor;
        [SerializeField] private Color _offColor;

        private void Start()
        {
            _checkbox.onValueChanged.AddListener(ChangeColor);
            ChangeColor(_checkbox.isOn);
        }

        private void OnDestroy()
        {
            _checkbox.onValueChanged.RemoveListener(ChangeColor);
        }

        private void ChangeColor(bool isChecked)
        {
            if (isChecked)
            {
                foreach (var image in _changeColorObjects)
                {
                    image.color = _onColor;
                }
            }
            else
            {
                foreach (var image in _changeColorObjects)
                {
                    image.color = _offColor;
                }
            }
        }
    }
}