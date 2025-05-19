using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Player
{
    /// <summary>
    /// <para>Компонент, контролирующий перемещение игрока при помощи клавиатуры и мышки.</para>
    /// </summary>
    public sealed class SDKPlayerControllerPC : AbstractPlayerController
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private float _mouseSensitivity = 2f;

        [Tooltip("Ограничение поворота головы персонажа вверх")] 
        [SerializeField] private float _lookUpRestriction = -50;

        [Tooltip("Ограничение поворота головы персонажа вниз")] 
        [SerializeField] private float _lookDownRestriction = 50;

        [SerializeField] private float _walkingSpeed = 6f;
        [SerializeField] private float _jumpHeight = 1.0f;
        
        private const float FREE_FALL_CONST = -9.87f;
        private CharacterController characterController;
        private bool isPlayerLanded;
        private Vector3 playerVelocity = Vector3.zero;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            characterController.Move(transform.position);
            Cursor.visible = false;
        }
        
        /// <summary>
        /// <inheritdoc cref="AbstractPlayerController.DeactivatePermanently"/>
        /// </summary>
        public override void DeactivatePermanently()
        {
            base.DeactivatePermanently();
            // На компоненте CinemachineBrain находятся так же AudioListener и MainCamera.
            // Их нужно отключить при отключении контроллера
            _mainCamera.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isActiveController)
            {
                Move4Direction();
                Jump();
                Rotate();
            }
        }

        private void Move4Direction()
        {
            Vector3 gravityMove = GetCharacterGravityMoveVector();
            Vector3 move = GetCharacterMoveVector(transform.forward);

            characterController.Move(_walkingSpeed * Time.deltaTime * move + gravityMove * Time.deltaTime);
        }

        private Vector3 GetCharacterGravityMoveVector()
        {
            playerVelocity.y = RecalculateFallVelocity(playerVelocity.y);
            return new Vector3(0, playerVelocity.y, 0);
        }

        private float RecalculateFallVelocity(float currentFallVelocity)
        {
            isPlayerLanded = characterController.isGrounded;
            if (isPlayerLanded && playerVelocity.y < 0)
            {
                return 0f;
            }
            
            return currentFallVelocity + FREE_FALL_CONST * Time.deltaTime;
        }

        private Vector3 GetCharacterMoveVector(Vector3 forwardVector)
        {
            float horizontalMove = GetHorizontalMove();
            float verticalMove = GetVerticalMove();

            return forwardVector * verticalMove + transform.right * horizontalMove;
        }

        private float GetHorizontalMove()
        {
            float result = 0;

            result = Input.GetKey(KeyCode.D) ? 1 : 0;
            result -= Input.GetKey(KeyCode.A) ? 1 : 0;
            return result;
        }

        private float GetVerticalMove()
        {
            float result = 0;
            
            result = Input.GetKey(KeyCode.W) ? 1 : 0;
            result -= Input.GetKey(KeyCode.S) ? 1 : 0;
            
            return result;
        }

        private void Rotate()
        {
            float horizontalRotation = Input.GetAxis("Mouse X");
            float verticalRotation = Input.GetAxis("Mouse Y");

            transform.Rotate(0, horizontalRotation * _mouseSensitivity, 0);
            _mainCamera.transform.Rotate(-verticalRotation * _mouseSensitivity, 0, 0);

            Vector3 currentRotation = _mainCamera.transform.localEulerAngles;

            if (currentRotation.x > 180)
            {
                currentRotation.x -= 360;
            }

            currentRotation.x = Mathf.Clamp(currentRotation.x, _lookUpRestriction, _lookDownRestriction);

            _mainCamera.transform.localRotation = Quaternion.Euler(currentRotation);
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isPlayerLanded)
            {
                playerVelocity.y += Mathf.Sqrt(_jumpHeight * -2.0f * FREE_FALL_CONST);
            }

            playerVelocity.y += FREE_FALL_CONST * Time.deltaTime;

            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }
}