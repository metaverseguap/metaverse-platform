using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Tablet.UI
{
    /// <summary>
    /// <para>Компонент, передающий планшету контент текущей комнаты.</para>
    /// </summary>
    public sealed class UITabletRoomContentSupplier : MonoBehaviour
    {
        [SerializeField] private GameObject _content;

        /// <summary>
        /// <para>Получить контент текущей комнаты.</para>
        /// </summary>
        /// <returns>контент текущей комнаты</returns>
        public GameObject GetRoomContent()
        {
            return _content;
        }
    }
}