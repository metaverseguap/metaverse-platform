using System.Collections;
using UnityEngine;

namespace Global.UI.LoadingForm
{
    /// <summary>
    /// <para>Скрипт управляющий включением формы загрузки.</para>
    /// </summary>
    public sealed class UILoadingForm : MonoBehaviour
    {
        /// <summary>
        /// <para>Включить форму загрузки, а затем перейти к загрузке указанного объекта.</para>
        /// </summary>
        /// <param name="enablingObject">загружаемый объект</param>
        public void EnableAfterLoading(GameObject enablingObject)
        {
            gameObject.SetActive(true);
            StartCoroutine(EnableAfterLoadingShowup(enablingObject));
        }
        
        private IEnumerator EnableAfterLoadingShowup(GameObject enablingObject)
        {
            // Ждем один кадр, чтобы форма загрузки отобразилась
            yield return null;
            
            enablingObject.SetActive(true);
            gameObject.SetActive(false);
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
            
            gameObject.SetActive(true);
        }
    }
}