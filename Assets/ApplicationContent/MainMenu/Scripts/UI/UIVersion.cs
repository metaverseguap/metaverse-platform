using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace MainMenu.UI
{
    /// <summary>
    /// <para>Отображение текущей версии приложения на ui.</para>
    ///
    /// <remarks>данный скрипт является примером подстановки собственных значений в локализованную строку</remarks>
    /// </summary>
    public sealed class UIVersion : MonoBehaviour
    {
        [SerializeField] private LocalizedString _localizedString;
        [SerializeField] private TMP_Text _versionLabel;

        private void Start()
        {
            _localizedString.Arguments = new object[] { Application.version };
            _localizedString.StringChanged += OnLocaleChanged;
        }

        private void OnDestroy()
        {
            _localizedString.StringChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged(string value)
        {
            _versionLabel.text = value;
        }
    }
}