using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Avatar.AvatarSetups
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
                Debug.LogError($"[{GetType().Name}]: Object not set");
                return true;
            }

            return false;
        }

        public override void SetUp(ref Animator avatarPrefab)
        {
            MoveObjectToBone(_object, avatarPrefab, _bone);
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