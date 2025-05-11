using Mirror;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetworkCore.MirrorNetworking.Synchronization.Transforms
{
    /// <summary>
    /// <para>Базовый класс синхронизации изменения положения на сервере.</para>
    /// </summary>
    [RequireComponent(typeof(MVNetworkOwnedObject))]
    public abstract class MVBaseNetworkTransform : NetworkBehaviour
    {
        [Header("Base Transform Synchronization")]
        [Tooltip("Точность сравнения локальной позиции и позиции сервера")]
        [Min(0.000001f)]
        [SerializeField] private float _positionThreshold = 0.001f;

        [Tooltip("Точность сравнения локального поворота и поворота сервера (в градусах)")]
        [Min(0.0001f)]
        [SerializeField] private float _rotationThreshold = 0.1f;

        [SyncVar]
        private Vector3 serverPosition;
        [SyncVar]
        private Quaternion serverRotation;
        [SyncVar]
        private bool isServerInit = false;

        private Vector3 lastSentPosition;
        private Quaternion lastSentRotation;
        private Vector3 lastUpdatePosition;
        private Quaternion lastUpdateRotation;

        /// <summary>
        /// Компонент, отслеживающий владение данным объектом.
        /// </summary>
        protected MVNetworkOwnedObject ownership;

        private void Start()
        {
            ownership = GetComponent<MVNetworkOwnedObject>();
            ownership.UseCustomTimer = true;
            
            OnStart();
        }

        /// <summary>
        /// <para><c>Start</c> для использования в подклассах.</para>
        /// </summary>
        protected virtual void OnStart()
        {
            // Ничего в базовом классе
        }

        private void FixedUpdate()
        {
            UpdateTransform();

            OnFixedUpdate();
        }
        
        /// <summary>
        /// <para><c>FixedUpdate</c> для использования в подклассах.</para>
        /// </summary>
        protected virtual void OnFixedUpdate()
        {
            // Ничего в базовом классе
        }

        private void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// <para><c>Update</c> для использования в подклассах.</para>
        /// </summary>
        protected virtual void OnUpdate()
        {
            // Ничего в базовом классе
        }
        
        private void UpdateTransform()
        {
            if (NeedTransformSync())
            {
                if (ownership.ObjectHasNoOwner() || ownership.AmIOwner())
                {
                    if (!ownership.AmIOwner())
                    {
                        ownership.CmdRequestOwnership();
                    }

                    ownership.ExtendOwnership();

                    if (ownership.AmIOwner())
                    {
                        lastSentPosition = lastUpdatePosition;
                        lastSentRotation = lastUpdateRotation;
                        CmdRequestMove(lastUpdatePosition, lastUpdateRotation);
                    }
                }
            }
            else
            {
                ownership.IncreaseOwnershipTimer(Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// <para>Изменить серверные координаты объекта.</para>
        /// </summary>
        /// <param name="position">положение объекта</param>
        /// <param name="rotation">поворот объекта</param>
        /// <param name="sender">игрок, запрашивающий владение объектом. Передается по умолчанию в сети mirror (не нужно указывать при вызове метода)</param>
        [Command(requiresAuthority = false)]
        private void CmdRequestMove(Vector3 position, Quaternion rotation, NetworkConnectionToClient sender = null)
        {
            serverPosition = position;
            serverRotation = rotation;
        }

        /// <summary>
        /// <para>Нужна ли синхронизация локальных координат с координатами сервера.</para>
        /// Проверяет, отличаются ли последние координаты установленные в <c>UpdateTransform</c>
        /// и координаты сервера
        /// </summary>
        /// <returns>true, если локальные координаты объекта отличаются от серверных</returns>
        protected bool NeedTransformSync()
        {
            if (!isServerInit)
            {
                return false;
            }
            
            bool positionChanged = Vector3.Distance(lastUpdatePosition, serverPosition) > _positionThreshold;
            bool rotationChanged = Quaternion.Angle(lastUpdateRotation, serverRotation) > _rotationThreshold;

            return positionChanged || rotationChanged;
        }

        /// <summary>
        /// <para>Получает интерполяцию между серверной позицией и указанной.</para>
        /// </summary>
        /// <param name="position">позиция, относительно которой выполняется интерполяция</param>
        /// <returns>интерполяция между серверной позицией и указанной</returns>
        protected Vector3 GetLerpServerPosition(Vector3 position)
        {
            return Vector3.Lerp(position, serverPosition, 0.2f);
        }

        /// <summary>
        /// <para>Получает интерполяцию между серверным поворотом и указанным.</para>
        /// </summary>
        /// <param name="rotation">поворот, относительно которого выполняется интерполяция</param>
        /// <returns>интерполяция между серверным поворотом и указанным</returns>
        protected Quaternion GetLerpServerRotation(Quaternion rotation)
        {
            return Quaternion.Slerp(rotation, serverRotation, 0.2f);
        }

        /// <summary>
        /// <para>Обновить локальное координаты объекта.</para>
        /// Сетевые обновления производятся реже, чем локальные.
        /// Что бы сетевое обновление использовало наиболее точные координаты - фиксируем локальные координаты,
        /// а во время сетевого обновления - отправляем последние зафиксированные локальные координаты
        /// </summary>
        /// <param name="position">положение объекта</param>
        /// <param name="rotation">поворот объекта</param>
        protected void UpdateTransform(Vector3 position, Quaternion rotation)
        {
            bool positionChanged = Vector3.Distance(position, lastSentPosition) > _positionThreshold;
            bool rotationChanged = Quaternion.Angle(rotation, lastSentRotation) > _rotationThreshold;

            if (positionChanged || rotationChanged)
            {
                lastUpdatePosition = position;
                lastUpdateRotation = rotation;
            }
        }

        /// <summary>
        /// <para>Установить серверное положение объекта.</para>
        /// <remarks>данный метод будет работать только на сервере</remarks>
        /// </summary>
        /// <param name="position">положение объекта</param>
        /// <param name="rotation">поворот объекта</param>
        protected void SetServerTransform(Vector3 position, Quaternion rotation)
        {
            if (!isServer)
            {
                return;
            }
            
            serverPosition = position;
            serverRotation = rotation;
            isServerInit = true;
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