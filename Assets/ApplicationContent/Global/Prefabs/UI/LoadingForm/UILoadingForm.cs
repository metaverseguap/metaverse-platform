using System.Collections;
using UnityEngine;

namespace Global.UI.LoadingForm
{
    /// <summary>
    /// <para>Скрипт управляющий включением формы загрузки.</para>
    /// </summary>
    public sealed class UILoadingForm : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingForm;

        private void Start()
        {
            _loadingForm.SetActive(false);
        }

        /// <summary>
        /// <para>Включить форму загрузки, а затем перейти к загрузке указанного объекта.</para>
        /// </summary>
        /// <param name="enablingObject">загружаемый объект</param>
        public void EnableAfterLoading(GameObject enablingObject)
        {
            _loadingForm.gameObject.SetActive(true);
            StartCoroutine(EnableAfterLoadingShowup(enablingObject));
        }
        
        private IEnumerator EnableAfterLoadingShowup(GameObject enablingObject)
        {
            // Ждем один кадр, чтобы форма загрузки отобразилась
            yield return null;
            
            enablingObject.SetActive(true);
            _loadingForm.gameObject.SetActive(false);
        }

        /// <summary>
        /// <para>Гарантированно показать форму загрузки.</para>
        /// </summary>
        public void EnableLoading()
        {
            StartCoroutine(AwaitFrameThenEnable());
        }
        
        private IEnumerator AwaitFrameThenEnable()
        {
            // Ждем один кадр, чтобы форма загрузки отобразилась
            yield return null;
            
            _loadingForm.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// <para>Гарантированно скрыть форму загрузки.</para>
        /// </summary>
        public void DisableLoading()
        {
            StartCoroutine(AwaitFrameThenDisable());
        }
        
        private IEnumerator AwaitFrameThenDisable()
        {
            // Ждем один кадр, чтобы форма загрузки скрылась
            yield return null;
            
            _loadingForm.gameObject.SetActive(false);
        }
    }
}