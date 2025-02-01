using Localization;

namespace Chat.UI
{
    /// <summary>
    /// <para>Отображение статуса чата на ui.</para>
    /// </summary>
    public sealed class UIChatStatus : UIAbstractSmartTextLocalization
    {
        /// <summary>
        /// <inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/>
        /// </summary>
        /// <returns><inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/></returns>
        protected override object[] StringArgs()
        {
            string chatStatus = "In development";

            // TODO: получить состояние чата активен/не активен и записать его в переменную chatStatus

            return new object[] { chatStatus };
        }
    }
}