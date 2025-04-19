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
        
        public void ShowTooltip()
        {
            _tooltipText.gameObject.SetActive(true);
            if (_tooltipCube != null)
            {
                _tooltipCube.gameObject.SetActive(true);
            }
        }

        public void HideTooltip()
        {
            _tooltipText.gameObject.SetActive(false);
            if (_tooltipCube != null)
            {
                _tooltipCube.gameObject.SetActive(false);
            }
        }
    }
}