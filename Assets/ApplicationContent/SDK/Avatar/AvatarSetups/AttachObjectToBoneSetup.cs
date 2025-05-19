using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Avatar.AvatarSetups
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
                Debug.LogError($"[{GetType().Name}]: Object not set");
                return true;
            }

            return false;
        }

        public override void SetUp(ref Animator avatarPrefab)
        {
            if (!avatarPrefab.isHuman)
            {
                Debug.LogError($"[{GetType().Name}]: Spawn avatar is not humanoid.");
                return;
            }
            
            Transform boneTransform = avatarPrefab.GetBoneTransform(_bone);
            _object.SetParent(boneTransform);
        }
    }
}