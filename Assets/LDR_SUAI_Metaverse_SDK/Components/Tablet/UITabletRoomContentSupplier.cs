using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Components.Tablet
{
    /// <summary>
    /// <para>Компонент, передающий планшету контент текущей комнаты.</para>
    /// </summary>
    public sealed class UITabletRoomContentSupplier : MonoBehaviour
    {
        [Tooltip(
            "Префаб контента текущей комнаты, отображаемый в планшете игрока. " +
            "Для создания данного префаба воспользуйтесь " +
            "`LDR_SUAI_Metaverse_SDK/Tools/TabletRoomContentCreator/CreateTabletRoomUITemplate.unity`"
        )]
        [SerializeField] private GameObject _content;

        /// <summary>
        /// <para>Получить контент текущей комнаты.</para>
        /// </summary>
        /// <returns>контент текущей комнаты</returns>
        public GameObject GetRoomContent()
        {
            if (_content == null)
            {
                Debug.LogError($"[{GetType().Name}]: The room page in the tablet is set to null");
            }
            
            return _content;
        }
    }
}