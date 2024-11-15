namespace NetworkCore.ServerInteraction.Type.Response.Details
{
    /// <summary>
    /// <para>Объект информирующего поля результата запроса.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class InfoDetails
    {
        /// <summary>
        /// Информация в строковом представлении.
        /// </summary>
        public string content { get; set; }
    }
}