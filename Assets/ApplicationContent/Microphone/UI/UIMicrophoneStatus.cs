using Localization;

namespace Microphone.UI
{
    /// <summary>
    /// <para>Отображение статуса микрофона на ui.</para>
    /// </summary>
    public sealed class UIMicrophoneStatus : UIAbstractSmartTextLocalization
    {
        /// <summary>
        /// <inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/>
        /// </summary>
        /// <returns><inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/></returns>
        protected override object[] StringArgs()
        {
            string microphoneStatus = "In development";

            // TODO: получить состояние микрофона вкл/выкл и записать его в переменную microphoneStatus

            return new object[] { microphoneStatus };
        }
    }
}