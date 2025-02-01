using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Localization
{
    /// <summary>
    /// <para>Абстрактный класс локализующий строку с переменными в тексте.</para>
    /// </summary>
    public abstract class UIAbstractSmartTextLocalization : MonoBehaviour
    {
        [SerializeField] private LocalizedString _localizedString;
        [SerializeField] private TMP_Text _text;

        private void Start()
        {
            _localizedString.Arguments = StringArgs();
            _localizedString.StringChanged += OnLocaleChanged;
            OnStart();
        }

        protected virtual void OnStart()
        {
            // Пустой в данном классе.
            // Будет использован в потомках вместо Start,
            // если потомкам нужно реализовать логику Start
        }

        /// <summary>
        /// <para>Метод создающий аргументы локализуемой строки.</para>
        /// Локализуемая строка имеет вид "Some text {0}, {1} and {2} ...".
        /// Данный метод должен вернуть массив объектов,
        /// которые будут, в соответствующем порядке, подставлены в строку.
        /// </summary>
        /// <returns>массив аргументов, подставляемых в строку</returns>
        protected abstract object[] StringArgs();

        private void OnEnable()
        {
            RefreshString();
            OnEnabled();
        }
        
        protected virtual void OnEnabled()
        {
            // Пустой в данном классе.
            // Будет использован в потомках вместо OnEnable,
            // если потомкам нужно реализовать логику OnEnable
        }

        private void OnDestroy()
        {
            _localizedString.StringChanged -= OnLocaleChanged;
            OnDestroyed();
        }
        
        protected virtual void OnDestroyed()
        {
            // Пустой в данном классе.
            // Будет использован в потомках вместо OnDestroy,
            // если потомкам нужно реализовать логику OnDestroy
        }

        private void OnLocaleChanged(string value)
        {
            _text.text = value;
        }

        /// <summary>
        /// <para>Метод осуществляет повторную локализацию строки, заново подставляе в нее аргументы.</para>
        /// </summary>
        protected void RefreshString()
        {
            _localizedString.Arguments = StringArgs();
            _localizedString.RefreshString();
        }
    }
}