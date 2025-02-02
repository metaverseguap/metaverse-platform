using Localization;
using UnityEngine;

namespace MainMenu.UI
{
    /// <summary>
    /// <para>Отображение текущей версии приложения на ui.</para>
    /// </summary>
    public sealed class UIVersion : UIAbstractSmartTextLocalization
    {
        /// <summary>
        /// <inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/>
        /// </summary>
        /// <returns><inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/></returns>
        protected override object[] StringArgs()
        {
            return new object[] { Application.version };
        }
    }
}