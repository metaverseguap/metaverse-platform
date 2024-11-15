namespace NetworkCore.ServerInteraction.Type.Request
{
    /// <summary>
    /// <para>Структура параметра передаваемого в Get запросе.</para>
    /// </summary>
    public struct GetParam
    {
        /// <summary>
        /// Название параметра.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Значение параметра.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="name">название параметра</param>
        /// <param name="value">значение параметра</param>
        private GetParam(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// <para>Фабричный метод.</para>
        /// </summary>
        /// <param name="name">название параметра</param>
        /// <param name="value">значение параметра</param>
        /// <returns>новый экземпляр структуры</returns>
        public static GetParam Form(string name, string value)
        {
            return new GetParam(name, value);
        }
    }
}