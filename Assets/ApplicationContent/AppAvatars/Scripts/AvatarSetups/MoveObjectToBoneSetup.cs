using AppAvatars.Containers;
using Global.Logger;
using UnityEngine;

namespace AppAvatars.AvatarSetups
{
    /// <summary>
    /// <para>Настройка аватара, которая перемещает объект к указанной кости аватара.</para>
    /// </summary>
    public sealed class MoveObjectToBoneSetup : AbstractAvatarSetup
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
            MoveObjectToBone(_object, avatarPrefabInfo.Prefab, _bone);
        }

        /// <summary>
        /// <para>Переместить объект к указанной кости аватара.</para>
        /// </summary>
        /// <param name="obj">перемещаемый объект</param>
        /// <param name="avatarPrefab">аниматор аватара, содержащий аватар</param>
        /// <param name="bone">кость, к которой необходимо переместить объект</param>
        public static void MoveObjectToBone(Transform obj, Animator avatarPrefab, HumanBodyBones bone)
        {
            Transform boneTransform = avatarPrefab.GetBoneTransform(bone);
            obj.position = boneTransform.position;
            obj.rotation = boneTransform.rotation;
        }
    }
}