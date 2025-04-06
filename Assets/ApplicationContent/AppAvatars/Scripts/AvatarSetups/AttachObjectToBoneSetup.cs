using Global.Logger;
using UnityEngine;

namespace AppAvatars.AvatarSetups
{
    /// <summary>
    /// <para>Настройка аватара, которая прикрепляет объект к указанной кости аватара.</para>
    /// </summary>
    public sealed class AttachObjectToBoneSetup : AbstractAvatarSetup
    {
        [SerializeField] private Transform _object;
        [SerializeField] private HumanBodyBones _bone;
        
        public override bool ComponentContainsErrors()
        {
            if (_object == null)
            {
                AppLogger.Error("Object not set");
                return true;
            }

            return false;
        }

        public override void SetUp(ref Animator avatarPrefab)
        {
            if (!avatarPrefab.isHuman)
            {
                AppLogger.Error("Spawn avatar is not humanoid.");
                return;
            }
            
            Transform boneTransform = avatarPrefab.GetBoneTransform(_bone);
            _object.SetParent(boneTransform);
        }
    }
}