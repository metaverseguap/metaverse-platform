using Mirror;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Offline
{
    /// <summary>
    /// <para>Компонент, который отключает сетевые функции объекта в офлайн режиме.</para>
    /// <remarks>если сетевые функции были отключены, то включить их можно только при перезагрузке сцены в онлайн режиме</remarks>
    /// </summary>
    public sealed class OfflineModeObject : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
            
            if (MVNetworkManager.IsOffline() && GetComponent<NetworkIdentity>() != null)
            {
                DisableNetworkComponents();
            }
            
            gameObject.SetActive(true);
        }

        private void DisableNetworkComponents()
        {
            var netId = GetComponent<NetworkIdentity>();
            if (netId != null)
            {
                DestroyImmediate(netId);
            }

            var netBehaviours = GetComponents<NetworkBehaviour>();
            for (int i = netBehaviours.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(netBehaviours[i]);
            }
        }
    }
}