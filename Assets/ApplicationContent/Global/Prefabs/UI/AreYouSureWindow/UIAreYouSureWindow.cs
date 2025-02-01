using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Global.UI.AreYouSureWindow
{
    /// <summary>
    /// <para>Окно "Вы уверены, что хотите совершить действие?".</para>
    /// </summary>
    public sealed class UIAreYouSureWindow : MonoBehaviour
    {
        [SerializeField] private UIAreYouSureLabel _label;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        
        private Vector3 _originalScale;

        private void Start()
        {
            _originalScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _noButton.onClick.AddListener(OnNoClick);
            transform.localScale = Vector3.zero;
        }

        private void OnDestroy()
        {
            _noButton.onClick.RemoveListener(OnNoClick);
        }

        private void OnNoClick()
        {
            transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// <para>Показать окно.</para>
        /// 
        /// Метод выведет окно с фразой "Вы уверены, что хотие сделать " + toDoWhat.
        /// </summary>
        /// <param name="toDoWhat">описание действия, которое запрашивается у пользователя</param>
        /// <param name="onSureClick">событие происходящее при подтверждении действия</param>
        public void ShowWindow(string toDoWhat, Action onSureClick)
        {
            _label.ChangeLabel(toDoWhat);
            transform.DOScale(_originalScale, 0.5f);
            _yesButton.onClick.AddListener(() =>
                {
                    onSureClick?.Invoke();
                    transform.localScale = Vector3.zero;
                    _yesButton.onClick.RemoveAllListeners();
                }
            );
        }
    }
}