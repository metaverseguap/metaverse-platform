using LDR.SUAI_Metaverse.SDK.Interactions;
using TMPro;
using UnityEngine;

namespace EmbeddedScenes.Adam.SceneObjects.ChangeTextCanvas
{
    /// <summary>
    /// <para>Пример ui, изменяющий текст в сети.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    public sealed class UIExampleChangeText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private IInteractionAccess access;
        
        private void Awake()
        {
            if (TryGetComponent(out IInteractionAccess externalAccessComponent))
            {
                access = externalAccessComponent;
            }
            else
            {
                access =  gameObject.AddComponent<DefaultInteractionAccess>();
            }
        }

        /// <summary>
        /// <para>Метод запрашивающий контроль над текстом.</para>
        /// </summary>
        /// <param name="owner">объект от чьего имени осуществляется запрос контроля</param>
        /// <returns>true, если удалось получить контроль над объектом</returns>
        public bool AcquireAccess(GameObject owner)
        {
            return IsInteractionAllowed(owner);
        }

        private bool IsInteractionAllowed(GameObject owner)
        {
            if (!access.IsInteractionAllowed() && !access.HasInteractionControl(owner))
            {
                return false;
            }

            if (!access.HasInteractionControl(owner))
            {
                access.AcquireControl(owner);
            }

            return true;
        }
        
        /// <summary>
        /// <para>Метод освобождающий контроль над текстом.</para>
        /// </summary>
        /// <param name="owner">объект от чьего имени осуществляется запрос на освобождение контроля</param>
        /// <returns>true, если удалось освободить контроль над объектом</returns>
        public bool ReleaseAccess(GameObject owner)
        {
            if (!access.HasInteractionControl(owner))
            {
                return false;
            }
            
            access.ReleaseControl();
            return true;
        }

        /// <summary>
        /// <para>Установить текст на UI, если над данным UI есть контроль.</para>
        /// </summary>
        /// <param name="owner">объект от чьего имени осуществляется запрос на изменение имени</param>
        /// <param name="text">текст</param>
        public void ChangeText(GameObject owner, string text)
        {
            if (!access.HasInteractionControl(owner))
            {
                return;
            }
            
            _text.text = text;
        }
    }
}