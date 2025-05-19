using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Avatar.Animations
{
    /// <summary>
    /// <para>Компонент контролирующий анимацию передвижения.</para>
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class WalkingAnimationController : MonoBehaviour
    {
        private static readonly int IS_WALKING = Animator.StringToHash("isWalking");
        private static readonly int WALKING_SPEED_MULTIPLIER = Animator.StringToHash("walkSpeedMultiplier");

        [SerializeField] private Transform _playerPosition;
        [SerializeField] private float _speedThreshold = 0.4f;

        [Range(0.1f, 5.0f)] 
        [SerializeField] private float _animationSpeedMultiplier = 1.25f;

        /// <summary>
        /// Transform игрока.
        /// </summary>
        public Transform PlayerPosition
        {
            get => _playerPosition;
            set => _playerPosition = value;
        }

        /// <summary>
        /// <para>Погрешность скорости перемещения.</para>
        ///
        /// Погрешность учитывается при переходе анимации от бездействия к бегу.
        /// Если скорость меньше погрешности переходим к бездействию.
        /// Если скорость больше погрешности переходим на бег
        /// </summary>
        public float SpeedThreshold
        {
            get => _speedThreshold;
            set => _speedThreshold = value;
        }

        /// <summary>
        /// Множитель скорости анимации.
        /// </summary>
        public float AnimationSpeedMultiplier
        {
            get => _animationSpeedMultiplier;
            set => _animationSpeedMultiplier = value;
        }

        private Animator animator;
        private Vector3 previousPosition;

        private void Start()
        {
            animator = GetComponent<Animator>();
            previousPosition = _playerPosition.position;
        }

        private void Update()
        {
            Vector3 playerLocalSpeed = GetPlayerLocalSpeed();

            SetAnimatorParameters(playerLocalSpeed);
        }

        private Vector3 GetPlayerLocalSpeed()
        {
            Vector3 currentPosition = _playerPosition.position;

            Vector3 playerSpeed = (currentPosition - previousPosition) / Time.deltaTime;
            playerSpeed.y = 0;

            Vector3 playerLocalSpeed = transform.InverseTransformDirection(playerSpeed);

            previousPosition = currentPosition;
            return playerLocalSpeed.normalized;
        }

        private void SetAnimatorParameters(Vector3 speed)
        {
            animator.SetBool(IS_WALKING, speed.magnitude > _speedThreshold);
            animator.SetFloat(WALKING_SPEED_MULTIPLIER, _animationSpeedMultiplier);
        }
    }
}