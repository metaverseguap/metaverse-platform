namespace Global.Containers
{
    /// <summary>
    /// <para>Контейнер имеющий поле Name.</para>
    /// </summary>
    public interface INamedContainer
    {
        /// <summary>
        /// Имя определяющие контейнер.
        /// </summary>
        public string Name { get; set; }
    }
}