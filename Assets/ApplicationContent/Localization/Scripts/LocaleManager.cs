using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Localization
{
    /// <summary>
    /// <para>Класс настроек языка приложения.</para>
    /// </summary>
    public sealed class LocaleManager : MonoBehaviour
    {
        private static bool isChangeLocaleNow;

        /// <summary>
        /// <para>Изменить язык приложения на следующий в спске.</para>
        /// </summary>
        public void SwitchApplicationLocale()
        {
            if (isChangeLocaleNow)
            {
                return;
            }

            isChangeLocaleNow = true;
            StartCoroutine(SwitchLocale());
        }

        /// <summary>
        /// <para>Изменить язык приложения на указанный.</para>
        /// </summary>
        /// <param name="locale">язык приложения</param>
        public void ChangeApplicationLocale(Locale locale)
        {
            if (isChangeLocaleNow)
            {
                return;
            }

            isChangeLocaleNow = true;
            StartCoroutine(SetLocale(locale));
        }

        private IEnumerator SetLocale(Locale locale)
        {
            isChangeLocaleNow = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = locale;
            isChangeLocaleNow = false;
        }

        private IEnumerator SwitchLocale()
        {
            isChangeLocaleNow = true;
            yield return LocalizationSettings.InitializationOperation;
            int id = LocalizationSettings.AvailableLocales.Locales.BinarySearch(LocalizationSettings.SelectedLocale);
            int count = LocalizationSettings.AvailableLocales.Locales.Count;
            int newId = (id + 1) % count;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[newId];
            isChangeLocaleNow = false;
        }
    }
}