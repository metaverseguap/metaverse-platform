using System;
using UnityEngine;

namespace Global.GUI_DIY
{
    /// <summary>
    /// <para>Прокручиваемый список с выбираемым элементом.</para>
    /// </summary>
    public sealed class GUISelectableList
    {
        public const float DEFAULT_MENU_ITEM_HEIGHT = 20f;
        public const float DEFAULT_MENU_ITEM_SPACE = 5f;
        
        public const int NO_ITEM_SELECTED = -1;

        private string title;
        private string subtitle;
        private string[] items;
        private int selectedItemIndex = NO_ITEM_SELECTED;

        private float labelHeight = 0;
        private readonly float listWidth;
        private readonly float listHeight;
        private float menuItemHeight = 20f;
        private float menuItemSpace = 5f;

        private readonly GUIStyle style;
        private Vector2 scrollPosition;

        /// <summary>
        /// Элементы списка.
        /// </summary>
        public string[] Items
        {
            get => items != null ? items : Array.Empty<string>();
            set => items = value;
        }

        /// <summary>
        /// Индекс выбранного элемента списка.
        /// </summary>
        public int SelectedItemIndex
        {
            get => selectedItemIndex;
            set => selectedItemIndex = value;
        }

        /// <summary>
        /// Выбранный элемент списка.
        /// </summary>
        public string SelectedItem => selectedItemIndex == NO_ITEM_SELECTED ? "" : Items[selectedItemIndex];

        /// <summary>
        /// Заголовок списка.
        /// </summary>
        public string Title
        {
            get => title;
            set => title = value;
        }

        /// <summary>
        /// Подзаголовок списка.
        /// </summary>
        public string Subtitle
        {
            get => subtitle;
            set => subtitle = value;
        }

        /// <summary>
        /// Высота пункта меню.
        /// </summary>
        public float MenuItemHeight
        {
            get => menuItemHeight;
            set => menuItemHeight = value;
        }

        /// <summary>
        /// Расстояние между пунктами меню.
        /// </summary>
        public float MenuItemSpace
        {
            get => menuItemSpace;
            set => menuItemSpace = value;
        }

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="listWidth">ширина списка</param>
        public GUISelectableList(float listWidth)
        {
            this.listWidth = listWidth;
            this.listHeight = DEFAULT_MENU_ITEM_HEIGHT;
            this.menuItemSpace = DEFAULT_MENU_ITEM_SPACE;
            style = GUI.skin.box;
        }

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="listWidth">ширина списка.</param>
        /// <param name="listHeight">высота списка</param>
        public GUISelectableList(float listWidth, float listHeight)
        {
            this.listWidth = listWidth;
            this.listHeight = listHeight;
            this.menuItemSpace = DEFAULT_MENU_ITEM_SPACE;
            style = GUI.skin.box;
        }

        /// <summary>
        /// <para>Нарисовать список в указанных координатах.</para>
        /// </summary>
        /// <param name="coordinates">координаты</param>
        /// <returns>индекс выбранного элемента списка</returns>
        public int Draw(Vector2 coordinates)
        {
            DrawTitle(coordinates);

            ScrolledList(coordinates);

            return selectedItemIndex;
        }

        private void DrawTitle(Vector2 coordinates)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Vector2 size = style.CalcSize(new GUIContent(title));
                labelHeight = size.y;
                GUI.Label(new Rect(coordinates.x, coordinates.y, size.x, size.y), title);
            }

            if (!string.IsNullOrEmpty(subtitle))
            {
                Vector2 size = style.CalcSize(new GUIContent(subtitle));
                labelHeight = size.y;
                GUI.Label(new Rect(coordinates.x, coordinates.y + labelHeight + menuItemSpace, size.x, size.y), subtitle);
            }
        }

        private void ScrolledList(Vector2 coordinates)
        {
            float yStart = coordinates.y + labelHeight * 2 + menuItemSpace;
            Rect viewRect = new Rect(coordinates.x, yStart, listWidth, listHeight);

            float contentHeight = labelHeight + Items.Length * (menuItemHeight + menuItemSpace);
            Rect contentRect = new Rect(coordinates.x, yStart, listWidth - 20, contentHeight);

            scrollPosition = GUI.BeginScrollView(viewRect, scrollPosition, contentRect);

            for (int index = 0; index < Items.Length; index++)
            {
                ListButton(index);
            }

            GUI.EndScrollView();
        }

        private void ListButton(int index)
        {
            Color defaultColor = GUI.backgroundColor;

            if (selectedItemIndex == index)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.black;
            }

            float y = index * (menuItemHeight + menuItemSpace) + labelHeight * 2 + menuItemSpace * 2;
            if (GUI.Button(new Rect(10, y, listWidth - 30, menuItemHeight), Items[index], style))
            {
                if (selectedItemIndex == index)
                {
                    selectedItemIndex = NO_ITEM_SELECTED;
                }
                else
                {
                    selectedItemIndex = index;
                }
            }

            GUI.backgroundColor = defaultColor;
        }
    }
}