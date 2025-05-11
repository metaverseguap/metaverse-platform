namespace Global.Containers
{
    /// <summary>
    /// <para>Контейнер имеющий метод Clone, возвращающий копию данного контейнера.</para>
    /// </summary>
    /// <typeparam name="T">тип контейнера</typeparam>
    public interface ICloneableContainer<out T>
    {
        /// <summary>
        /// <para>Получить копию данного контейнера.</para>
        /// </summary>
        /// <returns>копия данного контейнера</returns>
        T Clone();
    }
}