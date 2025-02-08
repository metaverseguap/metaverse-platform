using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Global.UI.ScrollList
{
    /// <summary>
    /// <para>Скрипт управляющий прокручиваемым списком с выбираемыми элементами.</para>
    /// </summary>
    public sealed class UIScrollList : MonoBehaviour
    {
        /// <summary>
        /// Событие происходящее при изменении значения элемента списка.
        /// </summary>
        public event UnityAction OnItemChangeValue;
        
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private bool _enableMultiselect = true;
        
        private readonly IList<Toggle> selectedItems = new List<Toggle>();
        private readonly IList<GameObject> items = new List<GameObject>();
        
        private RectTransform rectTransform;

        private void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        /// <para>Добавить выбираемый элемент списка с содеранием контент.</para>
        /// </summary>
        /// <param name="content">содержание выбираемого элемента списка.</param>
        public void AddItemWithContent(GameObject content)
        {
            GameObject item = Instantiate(_itemPrefab, _content);
            RectTransform itemRectTransform = item.GetComponent<RectTransform>();
            rectTransform = EnsureRectTransform();
            itemRectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - 30.0f, itemRectTransform.sizeDelta.y);
            items.Add(item);
            
            UIMarkerCheckElementContent itemContent = item.GetComponentInChildren<UIMarkerCheckElementContent>();
            Instantiate(content, itemContent.gameObject.transform);
            
            Toggle checkbox = item.GetComponentInChildren<Toggle>();
            selectedItems.Add(checkbox);

            checkbox.onValueChanged.AddListener((_) => SomeItemChangeValue(checkbox));
        }
        
        private RectTransform EnsureRectTransform()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            
            return rectTransform;
        }

        private void SomeItemChangeValue(Toggle checkbox)
        {
            if (checkbox.isOn && !_enableMultiselect)
            {
                foreach (var item in selectedItems)
                {
                    item.isOn = item == checkbox;
                }
            }

            OnItemChangeValue?.Invoke();
        }
        
        /// <summary>
        /// <para>Возвращает список индексов выбранных элементов.</para>
        /// </summary>
        /// <returns>список индексов выбранных элементов</returns>
        public IList<int> GetSelectedItemsIndexes()
        {
            IList<int> selected = new List<int>();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                if (selectedItems[i].isOn)
                {
                    selected.Add(i);
                }
            }

            return selected;
        }
        
        /// <summary>
        /// <para>Возвращает список выбранных элементов.</para>
        /// </summary>
        /// <returns>список выбранных элементов</returns>
        public IList<GameObject> GetSelectedItems()
        {
            IList<GameObject> selected = new List<GameObject>();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                if (selectedItems[i].isOn)
                {
                    UIMarkerCheckElementContent content = items[i].GetComponentInChildren<UIMarkerCheckElementContent>();
                    Transform item = content.gameObject.transform.GetChild(0);
                    selected.Add(item.gameObject);
                }
            }

            return selected;
        }

        /// <summary>
        /// <para>Выбрать элемент с указанным индексом.</para>
        /// </summary>
        /// <param name="index">индекс элемента</param>
        public void SelectItem(int index)
        {
            if (selectedItems.Count <= index)
            {
                return;
            }
            
            selectedItems[index].isOn = true;
        }

        /// <summary>
        /// <para>Сделать все элементы не выбранными.</para>
        /// </summary>
        public void DeselectAll()
        {
            foreach (var item in selectedItems)
            {
                item.isOn = false;
            }
        }

        /// <summary>
        /// <para>Очистить список.</para>
        /// </summary>
        public void Clear()
        {
            foreach (var toggle in selectedItems)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
            selectedItems.Clear();
            
            foreach (GameObject item in items)
            {
                Destroy(item);
            }
            items.Clear();
        }
    }
}