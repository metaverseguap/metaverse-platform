using System;

namespace NetworkCore.ServerInteraction.Type.LoginKey
{
    /// <summary>
    /// <para>Объект передачи данных ключа авторизации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class LoginKeyDTO
    {
        /// <summary>
        /// Ключ авторизации.
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Дата начала периода актуальности ключа.
        /// </summary>
        public DateTime dateFrom { get; set; }

        /// <summary>
        /// Дата окончания периода актуальности ключа.
        /// </summary>
        public DateTime dateTo { get; set; }
    }
}