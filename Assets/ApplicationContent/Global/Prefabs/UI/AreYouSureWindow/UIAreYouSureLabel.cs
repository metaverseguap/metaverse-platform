using Localization;

namespace Global.UI.AreYouSureWindow
{
    /// <summary>
    /// <para>Отображает надпись "Уверены ли вы что хотите...".</para>
    /// </summary>
    public sealed class UIAreYouSureLabel : UIAbstractSmartTextLocalization
    {
        private string toDoWhat = "";
        
        /// <summary>
        /// <inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/>
        /// </summary>
        /// <returns><inheritdoc cref="UIAbstractSmartTextLocalization.StringArgs"/></returns>
        protected override object[] StringArgs()
        {
            return new object[] { toDoWhat };
        }

        /// <summary>
        /// <para>Изменить заголовок.</para>
        ///
        /// Заголовок формируется из фразы "Вы уверены, что хотите " и входного параметра данного метода
        /// </summary>
        /// <param name="toDoWhat">описание действия, которое запрашивается у пользователя</param>
        public void ChangeLabel(string toDoWhat)
        {
            this.toDoWhat = toDoWhat;
            RefreshString();
        }
    }
}