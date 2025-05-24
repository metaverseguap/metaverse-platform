using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Components.Utils
{
    /// <summary>
    /// <para>Скрипт разворачивающий объект лицом к главной камере.</para>
    /// </summary>
    public sealed class RotateTowardsMainCamera : MonoBehaviour
    {
        private void Update()
        {
            GameObject mainCamera = GameObject.FindWithTag("MainCamera");
            if (mainCamera != null)
            {
                // поворачиваемся не на саму камеру, а в направлении куда смотрит камера
                Vector3 fromCameraToText = transform.position - mainCamera.transform.position;
                transform.LookAt(transform.position + fromCameraToText); 
            }
        }
    }
}