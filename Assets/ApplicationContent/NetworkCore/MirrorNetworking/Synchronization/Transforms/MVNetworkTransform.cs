using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization.Transforms
{
    /// <summary>
    /// <para>Компонент, синхронизирующий положение объекта с сервером.</para>
    /// Данный компонент синхронизирует объекты без RigidBody
    /// </summary>
    public sealed class MVNetworkTransform : MVBaseNetworkTransform
    {
        [Header("Object settings")]
        [SerializeField] private Transform _target;

        /// <summary>
        /// Синхронизируемый transform.
        /// </summary>
        public Transform Target
        {
            get => _target;
            set
            {
                _target = value;

                UpdateTransform(_target.transform.position, _target.transform.rotation);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (_target == null)
            {
                return;
            }

            if (isServer)
            {
                UpdateTransform(_target.transform.position, _target.transform.rotation);
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (_target == null)
            {
                return;
            }

            if (NeedTransformSync())
            {
                if (ownership.DoesObjectHaveAnotherOwner())
                {
                    _target.position = GetLerpServerPosition(_target.position);
                    _target.rotation = GetLerpServerRotation(_target.rotation);
                }
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            UpdateTransform(_target.position, _target.rotation);
        }
    }
}