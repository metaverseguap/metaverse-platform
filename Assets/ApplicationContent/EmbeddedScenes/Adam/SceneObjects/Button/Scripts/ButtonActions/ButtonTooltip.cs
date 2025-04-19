using TMPro;
using UnityEngine;

namespace Adam.SceneObjects.Button.ButtonActions
{
    /// <summary>
    /// <para>Компонент показывающий подсказку нажатия на кнопку.</para>
    /// </summary>
    public sealed class ButtonTooltip : MonoBehaviour, ITooltip
    {
        [SerializeField] private TMP_Text _buttonTooltip;
        [SerializeField] private GameObject _tooltipSphere;

        public void ShowTooltip()
        {
            _buttonTooltip.gameObject.SetActive(true);
            if (_tooltipSphere != null)
            {
                _tooltipSphere.gameObject.SetActive(true);
            }
        }

        public void HideTooltip()
        {
            _buttonTooltip.gameObject.SetActive(false);
            if (_tooltipSphere != null)
            {
                _tooltipSphere.gameObject.SetActive(false);
            }
        }
    }
}