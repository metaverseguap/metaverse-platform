using AppAvatars.Containers;
using NetworkCore.MirrorNetworking;
using UnityEngine;

namespace AppAvatars
{
    /// <summary>
    /// <para>Компонент устанавливающий аватар игрока через инспектор Unity.</para>
    ///
    /// <remarks>данный компонент нужен для офлайн дебага</remarks>
    /// </summary>
    [RequireComponent(typeof(AbstractPlayer))]
    public sealed class OfflinePlayerAvatar : MonoBehaviour
    {
        [Tooltip("Префаб аватара игрока")] 
        [SerializeField] private AvatarPrefabInfo _playerAvatar;
        [Tooltip("Установить аватар независимо от присутствия NetworkManager в сцене")]
        [SerializeField] private bool _forceAvatarSet;

        private void Start()
        {
            AbstractPlayer player = GetComponent<AbstractPlayer>();
            if (_forceAvatarSet || MVNetworkManager.IsOffline())
            {
                player.AvatarComponent.CreatePlayerFromAvatar(_playerAvatar);
            }
        }
    }
}