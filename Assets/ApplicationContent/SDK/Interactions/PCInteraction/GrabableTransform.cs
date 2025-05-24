using LDR.SUAI_Metaverse.SDK.NetworkSync;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LDR.SUAI_Metaverse.SDK.Interactions.PCInteraction
{
    /// <summary>
    /// <para>Компонент прикрепляющий объект к камере игрока при взаимодействии.</para>
    ///
    /// Данный компонент предназначен для объектов без RigidBody
    /// </summary>
    public sealed class GrabableTransform : MonoBehaviour, IInteractable
    {
        private const float RELEASE_DELAY = 0.5f;

        [Header("Position")]
        [Tooltip("Расстояние от игрока до объекта при взятии объекта.")]
        [SerializeField] private float _followDistance = 1.5f;

        [SerializeField] private Vector3 _offset = Vector3.zero;

        [Header("Movement")]
        [Tooltip("Скорость сглаживания перемещения объекта")]
        [SerializeField] private float _moveSmoothness = 8f;

        [Header("Interaction type")]
        [Tooltip("Взаимодействие через удержание клавиши")]
        [SerializeField] private bool _holdToGrab = false;

        [Tooltip("Продолжение взаимодействия через зрительный контакт")]
        [SerializeField] private bool _eyeContactGrab = false;

        private IInteractionAccess access;

        private bool isBeingDragged = false;
        private Transform ownerTransform;
        private float detachTimer = 0f;
        private Vector3 targetPosition;

        /// <summary>
        /// <inheritdoc cref="IInteractable.AllowHoldInteraction"/>
        /// </summary>
        public bool AllowHoldInteraction => _holdToGrab;

        private void Awake()
        {
            access = NetworkEnvironment.GetInteractionAccessor(gameObject);
        }

        private void OnValidate()
        {
            // Проверка корректной установки checkbox в инспекторе
            if (_eyeContactGrab && !_holdToGrab)
            {
                _eyeContactGrab = false;
            }
        }

        private void Update()
        {
            if (isBeingDragged && ownerTransform != null)
            {
                targetPosition = ownerTransform.position + ownerTransform.forward * _followDistance + _offset;

                transform.position = Vector3.Lerp(transform.position, targetPosition, _moveSmoothness * Time.deltaTime);

                if (_holdToGrab && _eyeContactGrab)
                {
                    detachTimer -= Time.deltaTime;
                    if (detachTimer <= 0f)
                    {
                        StopDragging();
                    }
                }
            }
        }

        /// <summary>
        /// <inheritdoc cref="IInteractable.Interact()"/>
        /// </summary>
        public void Interact()
        {
            // Ничего не делаем
        }

        /// <summary>
        /// <inheritdoc cref="IInteractable.Interact(GameObject)"/>
        /// </summary>
        /// <param name="owner"><inheritdoc cref="IInteractable.Interact(GameObject)"/></param>
        public void Interact(GameObject owner)
        {
            if (!IsInteractionAllowed(owner))
            {
                return;
            }

            if (_holdToGrab)
            {
                StartDragging(owner.transform);
                detachTimer = RELEASE_DELAY;
            }
            else
            {
                if (!isBeingDragged)
                {
                    StartDragging(owner.transform);
                }
                else
                {
                    StopDragging();
                }
            }

            Interact();
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
        /// <inheritdoc cref="IInteractable.StopInteraction"/>
        /// </summary>
        public void StopInteraction()
        {
            if (_holdToGrab && !_eyeContactGrab)
            {
                StopDragging();
            }
        }

        private void StartDragging(Transform newOwnerTransform)
        {
            isBeingDragged = true;

            if (!_holdToGrab || this.ownerTransform == null)
            {
                GameObject playerMainObject = newOwnerTransform.parent.parent.gameObject;
                Camera playerCamera = playerMainObject.GetComponentInChildren<Camera>();
                this.ownerTransform = playerCamera.transform;
            }
        }

        private void StopDragging()
        {
            isBeingDragged = false;
            ownerTransform = null;
            access.ReleaseControl();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (targetPosition != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(targetPosition, 0.1f);
                Handles.Label(targetPosition + Vector3.up * 0.3f, $"Local_{gameObject.name}");
            }
        }
#endif
    }
}