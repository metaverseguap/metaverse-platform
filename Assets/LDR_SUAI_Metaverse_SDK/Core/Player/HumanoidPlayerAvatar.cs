using LDR.SUAI_Metaverse.SDK.Core.Avatar.AvatarSetups;
using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Core.Player
{
    /// <summary>
    /// <para>Компонент, устанавливающий аватар игрока через инспектор Unity.</para>
    ///
    /// <remarks>данный компонент нужен для офлайн дебага</remarks>
    /// </summary>
    [RequireComponent(typeof(AbstractPlayer))]
    public sealed class HumanoidPlayerAvatar : MonoBehaviour
    {
        [Tooltip("Префаб аватара игрока")]
        [SerializeField] private Animator _playerAvatar;
        [Tooltip("Контроллер анимации аватара игрока")]
        [SerializeField] private AnimatorOverrideController _animatorController;

        private void Start()
        {
            AbstractPlayer player = GetComponent<AbstractPlayer>();

            if (!_playerAvatar.avatar.isHuman)
            {
                Debug.LogError($"[{GetType().Name}]: The avatar's skeleton is not humanoid");
                return;
            }

            _playerAvatar.runtimeAnimatorController = _animatorController;

            PlayerAvatar avatarComponent = player.AvatarComponent;

            avatarComponent.CreatePlayerFromAvatar(_playerAvatar);

            avatarComponent.SetAvatarVisibility(false);
        }
    }
}