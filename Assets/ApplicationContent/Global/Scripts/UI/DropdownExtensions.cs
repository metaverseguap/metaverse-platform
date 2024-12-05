using TMPro;

namespace Global.UI
{
    /// <summary>
    /// <para>Расширения для класса TMP_Dropdown.</para>
    /// </summary>
    public static class DropdownExtensions
    {
        /// <summary>
        /// <para>Сбросить значение выпадающего списка.</para>
        ///
        /// Установить значение выпадающего списка равное 0 и отображать на нем содержание нулевого элемента
        /// </summary>
        /// <param name="dropdown">выпадающий список</param>
        public static void Reset(this TMP_Dropdown dropdown)
        {
            dropdown.value = 0;
            dropdown.captionText.text = dropdown.options[dropdown.value].text;
        }

        /// <summary>
        /// <para>Установить значение выпадающего списка на указанное.</para>
        /// 
        /// Установить значение выпадающего списка равное указанному и отображать на нем содержание указанного элемента
        /// </summary>
        /// <param name="dropdown">выпадающий список</param>
        /// <param name="value">значение выпадающего списка</param>
        public static void ForceSetValue(this TMP_Dropdown dropdown, int value)
        {
            dropdown.value = value;
            dropdown.captionText.text = dropdown.options[dropdown.value].text;
        }
    }
}