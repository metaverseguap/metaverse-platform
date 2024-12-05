using Global.UI.ScrollList;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.ScrollList.Items
{
    /// <summary>
    /// <para>Элемент <see cref="UIScrollList"/>, хранящий ключ регистрации.</para>
    /// </summary>
    public sealed class UIRegistrationKeyItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _key;
        [SerializeField] private TMP_Text _organization;
        [SerializeField] private TMP_Text _serverRole;
        [SerializeField] private TMP_Text _role;
        [SerializeField] private TMP_Text _dateFrom;
        [SerializeField] private TMP_Text _dateTo;

        /// <summary>
        /// Ключ регистрации.
        /// </summary>
        public TMP_Text Key => _key;

        /// <summary>
        /// Организация создавшая ключ.
        /// </summary>
        public TMP_Text Organization => _organization;

        /// <summary>
        /// Роль пользователя на файловом сервере.
        /// </summary>
        public TMP_Text ServerRole => _serverRole;

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public TMP_Text Role => _role;

        /// <summary>
        /// Начало периода актуальности.
        /// </summary>
        public TMP_Text DateFrom => _dateFrom;

        /// <summary>
        /// Окончание периода актуальности.
        /// </summary>
        public TMP_Text DateTo => _dateTo;
    }
}