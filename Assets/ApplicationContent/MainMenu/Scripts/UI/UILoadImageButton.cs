using System.IO;
using Global.Converters;
using Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.UI
{
    /// <summary>
    /// <para>Кнопка загрузки изображения.</para>
    ///
    /// <remarks>Данная кнопка работает только в Unity Editor.</remarks>
    /// </summary>
    [RequireComponent(typeof(UIEditorButton))]
    public sealed class UILoadImageButton : MonoBehaviour
    {
// Данный код не должен попадать в сборку
#if UNITY_EDITOR

        private Sprite loadedImage;

        /// <summary>
        /// Загруженное изображение.
        /// </summary>
        public Sprite LoadedImage
        {
            get => loadedImage;
            set
            {
                EnsureButton();
                loadedImage = value;
                buttonImage.sprite = LoadedImage;
            }
        }

        private Button button;
        private Image buttonImage;

        private void OnEnable()
        {
            EnsureButton();
        }

        private void EnsureButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
                buttonImage = button.GetComponent<Image>();
                button.onClick.AddListener(LoadImage);
            }
        }

        private void LoadImage()
        {
            string path = OpenFile();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            byte[] fileData = File.ReadAllBytes(path);
            LoadedImage = DataConverter.SpriteFromRowData(fileData);
            LoadedImage.name = Path.GetFileNameWithoutExtension(path);
            buttonImage.sprite = LoadedImage;
        }

        private string OpenFile()
        {
            string title = LocalizationUtils.GetStringFromTable("MenuLocaleTable", "MainMenu.text.choose.image");
            return EditorUtility.OpenFilePanel(title, "", "png,jpg,jpeg");
        }
#endif
    }
}