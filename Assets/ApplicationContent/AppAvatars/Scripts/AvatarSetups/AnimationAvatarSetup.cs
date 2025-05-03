using AppAvatars.Animations;
using Global.Logger;
using UnityEngine;

namespace AppAvatars.AvatarSetups
{
    /// <summary>
    /// <para>Настройка аватара, которая добавляет аниматор к аватару.</para>
    /// </summary>
    public sealed class AnimationAvatarSetup : AbstractAvatarSetup
    {
        [Tooltip("Укажите положение игрока, которое будет изменяться в пространстве")]
        [SerializeField] private Transform _player;

        public override bool ComponentContainsErrors()
        {
            if (_player == null)
            {
                AppLogger.Error("Player transform is not set");
                return true;
            }

            return false;
        }

        public override void SetUp(ref Animator avatarPrefab)
        {
            WalkingAnimationController animationController = avatarPrefab.gameObject.AddComponent<WalkingAnimationController>();
            animationController.PlayerPosition = _player;
        }
    }
}