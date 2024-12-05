using System;
using NetworkCore.ServerInteraction.Type.Role;

namespace NetworkCore.ServerInteraction.Type.RegistrationKey
{
    /// <summary>
    /// <para>Объект передачи данных ключа регистрации.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class RegistrationKeyDTO
    {
        /// <summary>
        /// Ключ регистрации.
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

        /// <summary>
        /// Роль на файловом сервере.
        /// </summary>
        public SecurityRoleDTO securityRole { get; set; }

        /// <summary>
        /// Роль в приложении.
        /// </summary>
        public RoleDTO role { get; set; }

        /// <summary>
        /// Организация создавшая ключ.
        /// </summary>
        public string organization { get; set; }
    }
}