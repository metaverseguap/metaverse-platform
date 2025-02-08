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
        [Range(0.01f, 2)]
        [SerializeField] private float _activationDuration = 0.5f;
        
        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _noButton.onClick.AddListener(OnNoClick);
            transform.DOScale(Vector3.zero, 0.01f);
        }

        private void OnDestroy()
        {
            _noButton.onClick.RemoveListener(OnNoClick);
        }

        private void OnNoClick()
        {
            transform.DOScale(Vector3.zero, _activationDuration)
                .onComplete = () => gameObject.SetActive(false);
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
            gameObject.SetActive(true);
            _label.ChangeLabel(toDoWhat);
            transform.DOScale(_originalScale, _activationDuration);
            _yesButton.onClick.AddListener(() =>
                {
                    onSureClick?.Invoke();
                    _yesButton.onClick.RemoveAllListeners();
                    transform.DOScale(Vector3.zero, _activationDuration)
                        .onComplete = () => gameObject.SetActive(false);
                }
            );
        }
    }
}