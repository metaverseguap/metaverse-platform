using NetworkCore.MirrorNetworking.Containers.Store;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization.Transforms
{
    /// <summary>
    /// <para>Компонент, синхронизирующий положение объекта с сервером.</para>
    /// Данный компонент синхронизирует объекты имеющие RigidBody
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class MVNetworkRigidBody : MVBaseNetworkTransform
    {
        [Header("Object settings")]
        [SerializeField] private Rigidbody _target;
        
        private bool initialIsKinematic;

        /// <summary>
        /// Синхронизируемый Rigidbody.
        /// </summary>
        public Rigidbody Target
        {
            get => _target;
            set
            {
                _target = value;
                
                initialIsKinematic = _target.isKinematic;
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
                initialIsKinematic = _target.isKinematic;
                SetTransformFromCache();
                SetServerTransform(_target.transform.position, _target.transform.rotation);
            }
        }
        
        private void SetTransformFromCache()
        {
            NetworkDataStore store = MVNetworkManager.singleton.NetworkStore;
            CacheStore caches = store.HostMigration.Caches;
            if (caches.CurrentSceneCache.SyncTransformsCache.TryGetValue(netIdentity.sceneId, out var hostObjectTransform))
            {
                _target.transform.position = hostObjectTransform.Position;
                _target.transform.rotation = hostObjectTransform.Rotation;
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
                    if (!_target.isKinematic)
                    {
                        _target.isKinematic = true;
                    }
                    _target.MovePosition(GetLerpServerPosition(_target.position));
                    _target.MoveRotation(GetLerpServerRotation(_target.rotation));
                }
                else
                {
                    if (_target.isKinematic != initialIsKinematic)
                    {
                        _target.isKinematic = initialIsKinematic;
                    }
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