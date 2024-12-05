using Global.UI.ScrollList;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.ScrollList.Items
{
    /// <summary>
    /// <para>Элемент <see cref="UIScrollList"/>, хранящий ассет сцены.</para>
    /// </summary>
    public sealed class UISceneAssetItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_InputField _displayName;
        [SerializeField] private TMP_Dropdown _device;
        [SerializeField] private TMP_InputField _sortIndex;
        [SerializeField] private UILoadImageButton _loadImage;
        [SerializeField] private TMP_Text _status;

        /// <summary>
        /// Имя файла сцены.
        /// </summary>
        public TMP_Text Name => _name;

        /// <summary>
        /// Отображаемое имя сцены.
        /// </summary>
        public TMP_InputField DisplayName => _displayName;

        /// <summary>
        /// Устройство.
        /// </summary>
        public TMP_Dropdown Device => _device;

        /// <summary>
        /// Индекс сортировки.
        /// </summary>
        public TMP_InputField SortIndex => _sortIndex;

        /// <summary>
        /// Изображение сцены.
        /// </summary>
        public UILoadImageButton LoadImage => _loadImage;

        /// <summary>
        /// Статус (Локально/На сервере).
        /// </summary>
        public TMP_Text Status => _status;
    }
}