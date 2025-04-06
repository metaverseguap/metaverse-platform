using Global.UI.ScrollList;
using TMPro;
using UnityEngine;

namespace MainMenu.UI.ScrollListItems
{
    /// <summary>
    /// <para>Элемент <see cref="UIScrollList"/>, хранящий ассет аватара.</para>
    /// </summary>
    public sealed class UIAvatarAssetItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_InputField _displayName;
        [SerializeField] private TMP_Dropdown _animationControllerType;
        [SerializeField] private UILoadImageButton _loadImage;
        [SerializeField] private TMP_Text _status;

        /// <summary>
        /// Имя файла аватара.
        /// </summary>
        public TMP_Text Name => _name;

        /// <summary>
        /// Отображаемое имя аватара.
        /// </summary>
        public TMP_InputField DisplayName => _displayName;

        /// <summary>
        /// Пол аватара.
        /// </summary>
        public TMP_Dropdown AvatarAnimationControllerType => _animationControllerType;

        /// <summary>
        /// Изображение аватара.
        /// </summary>
        public UILoadImageButton LoadImage => _loadImage;

        /// <summary>
        /// Статус (Локально/На сервере).
        /// </summary>
        public TMP_Text Status => _status;
    }
}