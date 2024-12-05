using Global.UI.ScrollList;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.ScrollList.Items
{
    /// <summary>
    /// <para>Элемент <see cref="UIScrollList"/>, хранящий право (разрешение) пользователя.</para>
    /// </summary>
    public sealed class UIPermissionItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;

        /// <summary>
        /// Название права (разрешения).
        /// </summary>
        public TMP_Text Name => _name;
    }
}