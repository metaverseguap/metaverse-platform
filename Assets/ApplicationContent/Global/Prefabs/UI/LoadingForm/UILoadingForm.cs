using System.Collections;
using System.Threading.Tasks;
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
        /// 
        /// <remarks>Данный метод должен использоваться в методах async: <c>await EnableLoading();</c></remarks>
        /// </summary>
        public async Task EnableLoading()
        {
            gameObject.SetActive(true);
            await Task.Delay(1);
        }
    }
}