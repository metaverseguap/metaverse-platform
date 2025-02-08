using Global.UI.ScrollList;
using TMPro;
using UnityEngine;

namespace Player.Tablet.UI.ScrollListItems
{
    /// <summary>
    /// <para>Элемент <see cref="UIScrollList"/>, хранящий данные хоста.</para>
    /// </summary>
    public sealed class UIHostItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hostname;
        
        /// <summary>
        /// Имя хоста.
        /// </summary>
        public TMP_Text Hostname => _hostname;
    }
}