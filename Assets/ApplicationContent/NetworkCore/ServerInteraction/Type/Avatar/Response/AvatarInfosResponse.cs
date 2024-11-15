using System.Collections.Generic;
using NetworkCore.ServerInteraction.Type.Response;

namespace NetworkCore.ServerInteraction.Type.Avatar.Response
{
    /// <summary>
    /// <para>Объект результата запроса множества информаций об аватарах.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class AvatarInfosResponse : ResponseDetails
    {
        /// <summary>
        /// Список с информацией об аватарах
        /// </summary>
        public List<AvatarInfoRO> infoList { get; set; }
    }
}