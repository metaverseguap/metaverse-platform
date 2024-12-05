using AppAvatars.Containers;
using UnityEngine;

namespace AppAvatars
{
    /// <summary>
    /// <para>Компонент устанавливающий аватар игрока через инспектор Unity.</para>
    ///
    /// <remarks>данный компонент нужен для офлайн дебага</remarks>
    /// </summary>
    [RequireComponent(typeof(AbstractPlayer))]
    public sealed class PlayerAvatarSet : MonoBehaviour
    {
        [Tooltip("Префаб аватара игрока")] 
        [SerializeField] private AvatarPrefabInfo _playerAvatar;

        private void Start()
        {
            AbstractPlayer player = GetComponent<AbstractPlayer>();
            player.AvatarComponent.CreatePlayerFromAvatar(_playerAvatar);
        }
    }
}