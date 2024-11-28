using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Global.Logger
{
    /// <summary>
    /// <para>Класс для логгирования.</para>
    /// </summary>
    public static class AppLogger
    {
        /// <summary>
        /// <para>Логгировать сообщение об ошибке.</para>
        /// </summary>
        /// <param name="sender">класс отправляющий сообщение</param>
        /// <param name="message">сообщение</param>
        public static void Error(string message, [CallerFilePath] string sender = "")
        {
            Debug.LogError($"[{ClassName(sender)}]: {message}");
        }

        /// <summary>
        /// <para>Логгировать информирующие сообщение.</para>
        /// </summary>
        /// <param name="sender">класс отправляющий сообщение</param>
        /// <param name="message">сообщение</param>
        public static void Log(string message, [CallerFilePath] string sender = "")
        {
            Debug.Log($"[{ClassName(sender)}]: {message}");
        }

        /// <summary>
        /// <para>Логгировать предупреждение.</para>
        /// </summary>
        /// <param name="sender">объект отправляющий сообщение</param>
        /// <param name="message">сообщение</param>
        public static void Warning(string message, [CallerFilePath] string sender = "")
        {
            Debug.LogWarning($"[{ClassName(sender)}]: {message}");
        }

        private static string ClassName(string absoluteClassPath)
        {
            return Path.GetFileNameWithoutExtension(absoluteClassPath);
        }
    }
}