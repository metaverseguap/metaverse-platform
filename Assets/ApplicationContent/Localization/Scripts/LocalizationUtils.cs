using System.Collections.Generic;
using Global.Logger;
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
        /// Кеш таблиц локализации. Ключом является имя таблицы локализации.
        /// </summary>
        private static Dictionary<string, StringTable> localizationTablesCache = new Dictionary<string, StringTable>();

        /// <summary>
        /// <para>Получить локализованную строку по ключу из указанной таблицы.</para>
        /// </summary>
        /// <param name="tableName">имя таблицы</param>
        /// <param name="key">ключ</param>
        /// <returns>локализованная строка из указанной таблицы под указанным ключом или пустая строка, если в таблице не данного ключа</returns>
        public static string GetStringFromTable(string tableName, string key)
        {
            bool tableExists = EnsureLocalizationTable(tableName);
            if (!tableExists)
            {
                return "";
            }

            return GetLocalizedString(localizationTablesCache[tableName], key);
        }

        private static bool EnsureLocalizationTable(string tableName)
        {
            if (!localizationTablesCache.ContainsKey(tableName))
            {
                StringTable table = LocalizationSettings.StringDatabase.GetTable(tableName);
                if (table == null)
                {
                    AppLogger.Error($"Could not find table localization table named: {tableName}");
                    return false;
                }

                localizationTablesCache.Add(tableName, table);
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