using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Offline
{
    /// <summary>
    /// <para>Описание активности объекта в зависимости от подключения к сети.</para>
    /// </summary>
    [Serializable]
    public sealed class NetworkDependentObject
    {
        public GameObject Object;
        public bool Offline = true;
        public bool Online;
    }

    /// <summary>
    /// <para>Класс переключающий активность объекта, в зависимости от подключения к сети.</para>
    /// </summary>
    public sealed class NetworkObjectSwitcher : MonoBehaviour
    {
        [SerializeField] private List<NetworkDependentObject> _objects = new List<NetworkDependentObject>();

        private void OnEnable()
        {
            if (MVNetworkManager.IsOnline())
            {
                foreach (var obj in _objects)
                {
                    obj.Object.SetActive(obj.Online);
                }
            }
            else
            {
                foreach (var obj in _objects)
                {
                    obj.Object.SetActive(obj.Offline);
                }
            }
        }
    }
}