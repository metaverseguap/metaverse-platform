using UnityEngine;

namespace AppAvatars
{
    /// <summary>
    /// <para>Маркер для поиска части тела игрока.</para>
    /// </summary>
    public sealed class MarkerPlayerBodypart : MonoBehaviour
    {
        [SerializeField] private HumanBodyBones _attachToBone;

        /// <summary>
        /// Кость аватара, которая должна быть прикреплена к данной части тела.
        /// </summary>
        public HumanBodyBones AttachToBone => _attachToBone;
    }
}