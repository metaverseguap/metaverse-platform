namespace NetworkCore.ServerInteraction.Type.Response.Details
{
    /// <summary>
    /// <para>Объект поля ошибки результата запроса.</para>
    ///
    /// <remarks>имена свойств данного объекта должны соответствовать RO файлового сервера</remarks>
    /// </summary>
    public sealed class ErrorDetails
    {
        /// <summary>
        /// Код ошибки.
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string exceptionMessage { get; set; }
    }
}