using System.Collections.Generic;
using UnityEngine;

namespace MainMenu.UI
{
    /// <summary>
    /// <para>Класс переключающий панели на UI.</para>
    /// </summary>
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _previousPanels;
        [SerializeField] private List<GameObject> _nextPanels;
        
        /// <summary>
        /// <para>Переключить панели.</para>
        /// </summary>
        public void SwitchPanels()
        {
            SetActivePanel(_nextPanels, true);
            SetActivePanel(_previousPanels, false);
        }

        private static void SetActivePanel(List<GameObject> panels, bool active)
        {
            foreach (var panel in panels)
            {
                panel.SetActive(active);
            }
        }
    }
}