using System;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер ключа авторизации.</para>
    /// </summary>
    public sealed class LoginKeyInfo
    {
        /// <summary>
        /// Ключ авторизации.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Дата начала периода актуальности ключа.
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Дата окончания периода актуальности ключа.
        /// </summary>
        public DateTime DateTo { get; set; }
    }
}