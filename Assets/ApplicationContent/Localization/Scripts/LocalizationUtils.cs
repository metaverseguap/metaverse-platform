using System.Collections.Concurrent;
using System.Collections.Generic;
using Global.Logger;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Localization
{
    /// <summary>
    /// <para>Вспомогательные методы для работы с локализацией.</para>
    /// </summary>
    public static class LocalizationUtils
    {
        /// <summary>
        /// <para>Кеш таблиц локализации.</para>
        /// 
        /// Ключ первого словаря - локаль пользователя.
        /// Ключ вложенного словаря - имя таблицы.
        /// 
        /// </summary>
        private static ConcurrentDictionary<Locale, Dictionary<string, StringTable>> localizationTablesCache = new ConcurrentDictionary<Locale, Dictionary<string, StringTable>>();

        /// <summary>
        /// <para>Получить локализованную строку по ключу из указанной таблицы.</para>
        /// </summary>
        /// <param name="tableName">имя таблицы</param>
        /// <param name="key">ключ</param>
        /// <returns>локализованная строка из указанной таблицы под указанным ключом или пустая строка, если в таблице не данного ключа</returns>
        public static string GetStringFromTable(string tableName, string key)
        {
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            bool tableExists = EnsureLocalizationTable(currentLocale, tableName);
            if (!tableExists)
            {
                return "";
            }

            StringTable localizationTable = localizationTablesCache[currentLocale][tableName];
            return GetLocalizedString(localizationTable, key);
        }

        private static bool EnsureLocalizationTable(Locale locale, string tableName)
        {
            if (!localizationTablesCache.ContainsKey(locale) || !localizationTablesCache[locale].ContainsKey(tableName))
            {
                StringTable table = LocalizationSettings.StringDatabase.GetTable(tableName);
                if (table == null)
                {
                    AppLogger.Error($"Could not find table localization table named: {tableName}");
                    return false;
                }

                localizationTablesCache.TryAdd(locale, new Dictionary<string, StringTable>());
                localizationTablesCache[locale].Add(tableName, table);
            }

            return true;
        }

        private static string GetLocalizedString(StringTable table, string key)
        {
            StringTableEntry entry = table.GetEntry(key);
            return entry != null ? entry.LocalizedValue : "";
        }
    }
}