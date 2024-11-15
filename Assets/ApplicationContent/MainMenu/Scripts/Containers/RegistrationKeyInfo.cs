using System;
using RoleSystem.Core;

namespace MainMenu.Containers
{
    /// <summary>
    /// <para>Контейнер ключа регистрации.</para>
    /// </summary>
    public sealed class RegistrationKeyInfo
    {
        /// <summary>
        /// Ключ регистрации.
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

        /// <summary>
        /// Дата окончания периода актуальности ключа.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Роль на файловом сервере.
        /// </summary>
        public string ServerRole { get; set; }

        /// <summary>
        /// Роль в приложении.
        /// </summary>
        public AppRole Role { get; set; }
    }
}