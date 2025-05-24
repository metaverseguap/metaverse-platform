using LDR.SUAI_Metaverse.SDK.Core.Interactions;
using TMPro;
using UnityEngine;

namespace EmbeddedScenes.Adam.SceneObjects.Cube
{
    /// <summary>
    /// <para>Компонент показывающий подсказку взаимодействия с кубом.</para>
    /// </summary>
    public sealed class CubeTooltip : MonoBehaviour, ITooltip
    {
        [SerializeField] private TMP_Text _tooltipText;
        [SerializeField] private GameObject _tooltipCube;
        
        /// <summary>
        /// <inheritdoc cref="ITooltip.ShowTooltip"/>
        /// </summary>
        public void ShowTooltip()
        {
            _tooltipText?.gameObject.SetActive(true);
            _tooltipCube?.gameObject.SetActive(true);
        }

        /// <summary>
        /// <inheritdoc cref="ITooltip.HideTooltip"/>
        /// </summary>
        public void HideTooltip()
        {
            _tooltipText?.gameObject.SetActive(false);
            _tooltipCube?.gameObject.SetActive(false);
        }
    }
}