using Global.Logger;
using NetworkCore.MirrorNetworking;
using Player.EmbeddedPlayers;
using UnityEngine;

namespace AppAvatars
{
    /// <summary>
    /// <para>Компонент устанавливающий аватар игрока через инспектор Unity.</para>
    ///
    /// <remarks>данный компонент нужен для офлайн дебага</remarks>
    /// </summary>
    [RequireComponent(typeof(AbstractPlayer))]
    public sealed class OfflineHumanoidPlayerAvatar : MonoBehaviour
    {
        [Tooltip("Префаб аватара игрока")]
        [SerializeField] private Animator _playerAvatar;
        [Tooltip("Контроллер анимации аватара игрока")]
        [SerializeField] private AnimatorOverrideController _animatorController;
        
        [Tooltip("Установить аватар независимо от присутствия NetworkManager в сцене")]
        [SerializeField] private bool _forceAvatarSet;

        private void Start()
        {
            AbstractPlayer player = GetComponent<AbstractPlayer>();
            if (_forceAvatarSet || MVNetworkManager.IsOffline())
            {
                if (!_playerAvatar.avatar.isHuman)
                {
                    AppLogger.Error("The avatar's skeleton is not humanoid");
                    return;
                }
                
                _playerAvatar.runtimeAnimatorController = _animatorController;

                player.AvatarComponent.CreatePlayerFromAvatar(_playerAvatar);
            }
        }
    }
}