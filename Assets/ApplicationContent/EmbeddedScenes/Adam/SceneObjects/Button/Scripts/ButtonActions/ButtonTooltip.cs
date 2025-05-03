using LDR.SUAI_Metaverse.SDK.Interactions;
using TMPro;
using UnityEngine;

namespace Adam.SceneObjects.Button.ButtonActions
{
    /// <summary>
    /// <para>Компонент, показывающий подсказку нажатия на кнопку.</para>
    /// </summary>
    public sealed class ButtonTooltip : MonoBehaviour, ITooltip
    {
        [SerializeField] private TMP_Text _buttonTooltip;
        [SerializeField] private GameObject _tooltipSphere;

        /// <summary>
        /// <inheritdoc cref="ITooltip.ShowTooltip"/>
        /// </summary>
        public void ShowTooltip()
        {
            _buttonTooltip?.gameObject.SetActive(true);
            _tooltipSphere?.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// <inheritdoc cref="ITooltip.HideTooltip"/>
        /// </summary>
        public void HideTooltip()
        {
            _buttonTooltip?.gameObject.SetActive(false);
            _tooltipSphere?.gameObject.SetActive(false);
        }
    }
}