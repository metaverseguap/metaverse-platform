using EmbeddedScenes.Adam.SceneObjects.ChangeTextCanvas;
using LDR.SUAI_Metaverse.SDK.NetworkSync;
using UnityEngine;
using UnityEngine.UI;

namespace Adam.Tablet
{
    /// <summary>
    /// <para>Компонент, получающий имя текущего игрока.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    public sealed class UIExamplePlayerNameSender : MonoBehaviour
    {
        [SerializeField] private UIExampleChangeText _standWithText;
        [SerializeField] private Button _releaseAccessButton;
        [SerializeField] private Button _changeTextButton;

        private string playerName;

        private void Start()
        {
            playerName = NetworkEnvironment.GetUsername();
        }

        /// <summary>
        /// <para>Метод запрашивающий контроль над текстом стенда.</para>
        /// </summary>
        public void AcquireStandAccess()
        {
            if (_standWithText.AcquireAccess(gameObject))
            {
                _releaseAccessButton.interactable = true;
                _changeTextButton.interactable = true;
            }
            else
            {
                _releaseAccessButton.interactable = false;
                _changeTextButton.interactable = false;
            }
        }
        
        /// <summary>
        /// <para>Метод освобождающий контроль над текстом стенда.</para>
        /// </summary>
        public void ReleaseStandAccess()
        {
            if (_standWithText.ReleaseAccess(gameObject))
            {
                _releaseAccessButton.interactable = false;
                _changeTextButton.interactable = false;
            }
        }

        /// <summary>
        /// <para>Изменить текст на стенде.</para>
        /// </summary>
        public void SetPlayerNameToStand()
        {
            _standWithText.ChangeText(gameObject, playerName);
        }
    }
}