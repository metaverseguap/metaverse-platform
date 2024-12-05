using AppAvatars.Containers;
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

        public override void SetUp(ref AvatarPrefabInfo avatarPrefabInfo)
        {
            Transform boneTransform = avatarPrefabInfo.Prefab.GetBoneTransform(_bone);
            _object.SetParent(boneTransform);
        }
    }
}