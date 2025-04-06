using UnityEngine;

namespace AppAvatars.AvatarSetups
{
    /// <summary>
    /// <para>Абстрактная настройка аватара.</para>
    ///
    /// Наследники данного класса производят манипуляции над созданным аватаром.
    /// </summary>
    public abstract class AbstractAvatarSetup : MonoBehaviour
    {
        /// <summary>
        /// <para>Проверка ошибок компонента, перед настройкой аватара.</para>
        /// </summary>
        /// <returns>true если компонент содержит ошибки, false если компонент находиться в валидном состоянии и может произвести настройку</returns>
        public virtual bool ComponentContainsErrors()
        {
            return false;
        }

        /// <summary>
        /// <para>Настройка заспавненного аватара.</para>
        /// </summary>
        /// <param name="avatarPrefab">заспавненный аватар</param>
        public abstract void SetUp(ref Animator avatarPrefab);
    }
}