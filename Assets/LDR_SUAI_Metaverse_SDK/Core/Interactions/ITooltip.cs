namespace LDR.SUAI_Metaverse.SDK.Core.Interactions
{
    /// <summary>
    /// <para>Интерфейс подсказки объекта.</para>
    /// </summary>
    public interface ITooltip
    {
        /// <summary>
        /// <para>Показать подсказку интерактивного объекта.</para>
        /// </summary>
        void ShowTooltip();

        /// <summary>
        /// <para>Скрыть подсказку интерактивного объекта.</para>
        /// </summary>
        void HideTooltip();
    }
}