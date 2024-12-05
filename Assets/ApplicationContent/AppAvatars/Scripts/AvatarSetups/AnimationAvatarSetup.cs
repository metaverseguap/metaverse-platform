using System.Collections.Generic;
using AppAvatars.Animations;
using AppAvatars.Containers;
using Global.Logger;
using UnityEngine;

namespace AppAvatars.AvatarSetups
{
    /// <summary>
    /// <para>Настройка аватара, которая добавляет аниматор к аватару.</para>
    /// </summary>
    public sealed class AnimationAvatarSetup : AbstractAvatarSetup
    {
        [Tooltip("Анимация будет изменяться в зависимости от изменения положения игрока. Укажите положение игрока, которое будет изменяться в пространстве")]
        [SerializeField] private Transform _player;
        [SerializeField] private List<AnimatorControllerInfo> _animatorControllers;

        public override bool ComponentContainsErrors()
        {
            if (_player == null)
            {
                AppLogger.Error("Player transform is not set");
                return true;
            }
            
            if (_animatorControllers.Count == 0)
            {
                AppLogger.Error("No animator controllers found");
                return true;
            }

            for (int i = 0; i < _animatorControllers.Count; i++)
            {
                if (_animatorControllers[i].Controller == null)
                {
                    AppLogger.Error($"Animator controller [{i}] reference is null");
                    return true;
                }
            }

            return false;
        }

        public override void SetUp(ref AvatarPrefabInfo avatarPrefabInfo)
        {
            foreach (var animatorController in _animatorControllers)
            {
                if (animatorController.ForGender == avatarPrefabInfo.ForGender)
                {
                    CreateAnimationController(animatorController, ref avatarPrefabInfo);
                    return;
                }
            }
        }

        private void CreateAnimationController(AnimatorControllerInfo animatorController, ref AvatarPrefabInfo avatarPrefabInfo)
        {
            avatarPrefabInfo.Prefab.runtimeAnimatorController = animatorController.Controller;
            WalkingAnimationController animationController = avatarPrefabInfo.Prefab.gameObject.AddComponent<WalkingAnimationController>();
            animationController.PlayerPosition = _player;
        }
    }
}