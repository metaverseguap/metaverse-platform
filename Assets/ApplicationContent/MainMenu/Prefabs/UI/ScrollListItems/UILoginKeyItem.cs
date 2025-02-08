using Global.UI.ScrollList;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.ScrollListItems
{
    /// <summary>
    /// <para>Элемент <see cref="UIScrollList"/>, хранящий ключ авторизации.</para>
    /// </summary>
    public sealed class UILoginKeyItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _key;
        [SerializeField] private TMP_Text _dateFrom;
        [SerializeField] private TMP_Text _dateTo;

        /// <summary>
        /// Ключ авторизации.
        /// </summary>
        public TMP_Text Key => _key;

        /// <summary>
        /// Дата начала периода актуальности.
        /// </summary>
        public TMP_Text DateFrom => _dateFrom;

        /// <summary>
        /// Дата окончания периода актуальности.
        /// </summary>
        public TMP_Text DateTo => _dateTo;
    }
}