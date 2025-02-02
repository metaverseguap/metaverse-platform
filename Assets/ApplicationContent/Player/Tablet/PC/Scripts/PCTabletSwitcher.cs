using System.Collections;
using Cinemachine;
using Player.EmbeddedPlayers.PC;
using UnityEngine;

namespace Player.Tablet.PC
{
    /// <summary>
    /// <para>Скрипт отвечающий за появление/исчезновение планшета игрока.</para>
    /// </summary>
    public sealed class PCTabletSwitcher : MonoBehaviour
    {
        [SerializeField] private KeyCode _tabletSwitchKey = KeyCode.F2;

        [Header("Controllers")]
        [SerializeField] private PlayerControllerPC _player;
        [SerializeField] private PCTabletController _tablet;

        [Header("Cinemachine")]
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        [SerializeField] private CinemachineVirtualCamera _playerCamera;
        [SerializeField] private CinemachineVirtualCamera _tabletCamera;

        private bool isTabletActive;

        private void Start()
        {
            isTabletActive = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_tabletSwitchKey))
            {
                SwitchTablet();
            }
        }

        /// <summary>
        /// <para>Вкл/выкл планшет.</para>
        /// </summary>
        public void SwitchTablet()
        {
            // Если предыдущее переключение еще идет
            if (_cinemachineBrain.IsBlending)
            {
                return;
            }
            
            isTabletActive = !isTabletActive;

            if (isTabletActive)
            {
                _tablet.gameObject.SetActive(true);
                _playerCamera.Priority = CameraControlConstants.PRIORITY_CAMERA_OFF;
                _tabletCamera.Priority = CameraControlConstants.PRIORITY_SELECTED_CAMERA;
                _player.ActiveController = false;
                Cursor.visible = true;
            }
            else
            {
                _tabletCamera.Priority = CameraControlConstants.PRIORITY_CAMERA_OFF;
                _playerCamera.Priority = CameraControlConstants.PRIORITY_SELECTED_CAMERA;
                _player.ActiveController = true;
                Cursor.visible = false;
            }
            
            StartCoroutine(WaitForBlendToEnd());
        }
        
        private IEnumerator WaitForBlendToEnd()
        {
            // Ждем один кадр, что бы запустился блендинг
            yield return null;
            
            // Ожидаем завершения перехода
            while (_cinemachineBrain.IsBlending)
            {
                yield return null; // Ждем завершения перехода
            }
            
            if (_playerCamera.Priority > _tabletCamera.Priority)
            {
                _tablet.gameObject.SetActive(false);
            }
        }
    }
}