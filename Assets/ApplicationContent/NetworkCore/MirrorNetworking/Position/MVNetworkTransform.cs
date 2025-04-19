using Mirror;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetworkCore.MirrorNetworking.Position
{
    /// <summary>
    /// <para>Компонент синхронизирующий положение объекта с сервером.</para>
    /// </summary>
    public sealed class MVNetworkTransform : NetworkBehaviour
    {
        [SerializeField] private Transform _target;

        [Tooltip("Точность сравнения локальной позиции и позиции сервера")]
        [SerializeField] private float _positionThreshold = 0.001f;

        [Tooltip("Точность сравнения локального поворота и поворота сервера (в градусах)")]
        [SerializeField] private float _rotationThreshold = 0.1f;

        [SerializeField] private float _ownershipTime = 0.2f;

        /// <summary>
        /// Синхронизируемый transform.
        /// </summary>
        public Transform Target
        {
            get => _target;
            set
            {
                _target = value;

                serverPosition = _target.transform.position;
                serverRotation = _target.transform.rotation;

                _target.transform.position = serverPosition;
                _target.transform.rotation = serverRotation;
                lastSentPosition = serverPosition;
                lastSentRotation = serverRotation;
            }
        }
        
        [SyncVar] private Vector3 serverPosition;
        [SyncVar] private Quaternion serverRotation;
        [SyncVar] private bool hasOwner = false;
        [SyncVar] private int ownerId;
        
        private Vector3 lastSentPosition;
        private Quaternion lastSentRotation;
        private Vector3 lastUpdatePosition;
        private Quaternion lastUpdateRotation;
        
        private float ownershipTimer = 0f;
        private int? myConnectionId = null;

        private void Start()
        {
            if (_target == null)
            {
                return;
            }

            if (isServer)
            {
                serverPosition = _target.transform.position;
                serverRotation = _target.transform.rotation;
            }

            _target.transform.position = serverPosition;
            _target.transform.rotation = serverRotation;
            lastSentPosition = serverPosition;
            lastSentRotation = serverRotation;
            lastUpdatePosition = serverPosition;
            lastUpdateRotation = serverRotation;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            CmdRequestMyConnectionId();
        }

        private void FixedUpdate()
        {
            UpdateTransform();
        }
        
        private void UpdateTransform()
        {
            if (_target == null)
            {
                return;
            }
            
            bool positionChanged = Vector3.Distance(lastUpdatePosition, serverPosition) > _positionThreshold;
            bool rotationChanged = Quaternion.Angle(lastUpdateRotation, serverRotation) > _rotationThreshold;
            if (positionChanged || rotationChanged)
            {
                if (DoesObjectHaveAnotherOwner())
                {
                    _target.position = Vector3.Lerp(_target.position, serverPosition, 0.2f);
                    _target.rotation = Quaternion.Slerp(_target.rotation, serverRotation, 0.2f);
                }
                else
                {
                    CmdBecomeOwner();
                    if (AmIOwner())
                    {
                        ownershipTimer = _ownershipTime;
                        
                        lastSentPosition = lastUpdatePosition;
                        lastSentRotation = lastUpdateRotation;
                        CmdRequestMove(lastUpdatePosition, lastUpdateRotation);
                    }
                }
            }
            else
            {
                if (AmIOwner())
                {
                    ownershipTimer -= Time.fixedDeltaTime;
                    if (ownershipTimer <= 0f)
                    {
                        CmdStopOwnership();
                    }
                }
            }
        }

        private void Update()
        {
            Vector3 currentPosition = _target.position;
            Quaternion currentRotation = _target.rotation;
            
            bool positionChanged = Vector3.Distance(currentPosition, lastSentPosition) > _positionThreshold;
            bool rotationChanged = Quaternion.Angle(currentRotation, lastSentRotation) > _rotationThreshold;
            
            if (positionChanged || rotationChanged)
            {
                lastUpdatePosition = currentPosition;
                lastUpdateRotation = currentRotation;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdRequestMyConnectionId(NetworkConnectionToClient sender = null)
        {
            if (sender == null)
            {
                sender = connectionToClient;
            }

            int connectionId = sender.connectionId;
        
            // Отправляем обратно клиенту
            TargetReceiveConnectionId(sender, connectionId);
        }

        // Этот метод будет вызван только на целевом клиенте
        [TargetRpc]
        private void TargetReceiveConnectionId(NetworkConnection target, int connectionId)
        {
            myConnectionId = connectionId;
        }
        
        [Command(requiresAuthority = false)]
        private void CmdBecomeOwner(NetworkConnectionToClient sender = null)
        {
            if (sender == null)
            {
                sender = connectionToClient; 
            }

            if (hasOwner && sender.connectionId != ownerId)
            {
                return;
            }

            hasOwner = true;
            if (ownerId != sender.connectionId)
            {
                if (netIdentity.connectionToClient != null)
                {
                    netIdentity.RemoveClientAuthority();
                }

                if (netIdentity.connectionToClient != sender)
                {
                    netIdentity.AssignClientAuthority(sender);
                }

                ownerId = sender.connectionId;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdStopOwnership(NetworkConnectionToClient sender = null)
        {
            if (sender == null)
            {
                sender = connectionToClient;
            }

            if (!hasOwner || sender.connectionId != ownerId)
            {
                return;
            }

            hasOwner = false;
            if (netIdentity.connectionToClient != null)
            {
                netIdentity.RemoveClientAuthority();
            }
        }
        
        [Command(requiresAuthority = false)]
        private void CmdRequestMove(Vector3 newPos, Quaternion newRot, NetworkConnectionToClient sender = null)
        {
            serverPosition = newPos;
            serverRotation = newRot;
        }

        private bool DoesObjectHaveAnotherOwner()
        {
            return hasOwner && !AmILastOwner();
        }

        private bool AmIOwner()
        {
            return hasOwner && AmILastOwner();
        }

        private bool AmILastOwner()
        {
            return myConnectionId != null && ownerId == myConnectionId;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (serverPosition != null)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
                Gizmos.DrawCube(serverPosition, Vector3.one * 0.2f);
                Handles.Label(serverPosition + Vector3.up * 0.3f, $"Server_{gameObject.name}");
            }
        }
#endif
    }
}