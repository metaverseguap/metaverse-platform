using LDR.SUAI_Metaverse.SDK.Core.Avatar.Animations;
using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Core.Avatar.AvatarSetups
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
                Debug.LogError($"[{GetType().Name}]: Player transform is not set");
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