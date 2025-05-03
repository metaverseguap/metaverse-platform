using Mirror;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization
{
    /// <summary>
    /// <para>Класс объекта имеющего владельца в сети Mirror.</para>
    /// </summary>
    public class MVNetworkOwnedObject : NetworkBehaviour
    {
        [Header("Ownership")]
        [Tooltip("Время, на которое происходит передача владения объектом по запросу")]
        [Min(0.02f)]
        [SerializeField] private float _ownershipTime = 0.2f;

        [SyncVar]
        private bool hasOwner = false;
        [SyncVar]
        private int ownerId;

        private float ownershipTimer = 0f;
        private int? myConnectionId = null;

        /// <summary>
        /// Время, на которое происходит передача владения объектом по запросу.
        /// </summary>
        public float OwnershipTime
        {
            get => _ownershipTime;
            set => _ownershipTime = value;
        }
        
        /// <summary>
        /// <para>Использовать собственный отсчет таймера владения объектом.</para>
        /// По умолчанию, данный компонент сам отсчитывает время владения объектом,
        /// но если наследник имеет собственные критерии изменения таймера владения,
        /// то необходимо установить данный параметр в true и использовать метод <c>IncreaseOwnershipTimer</c>
        /// для отсчета времени владения
        /// </summary>
        public bool UseCustomTimer { get; set; }

        public override void OnStartClient()
        {
            base.OnStartClient();

            CmdRequestMyConnectionId();
        }

        private void FixedUpdate()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            if (UseCustomTimer)
            {
                return;
            }

            IncreaseOwnershipTimer(Time.fixedDeltaTime);
        }

        /// <summary>
        /// <para>Уменьшает таймер владения объектом на заданное количество времени.</para>
        /// Когда таймер истечет - владение объектом прекратится.
        /// </summary>
        /// <param name="tick">величина времени, на которое уменьшается таймер за один вызов метода (обычно равна <see cref="Time.fixedDeltaTime"/>)</param>
        /// <remarks> Для использования этого метода в подклассах, необходимо установить <c>customTimer == true</c> </remarks>
        public void IncreaseOwnershipTimer(float tick)
        {
            if (!AmIOwner())
            {
                return;
            }

            ownershipTimer -= tick;
            if (ownershipTimer <= 0f)
            {
                CmdStopOwnership();
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

        /// <summary>
        /// <para>Запросить владение объектом.</para>
        /// </summary>
        /// <param name="sender">игрок, запрашивающий владение объектом. Передается по умолчанию в сети mirror (не нужно указывать при вызове метода)</param>
        [Command(requiresAuthority = false)]
        public void CmdRequestOwnership(NetworkConnectionToClient sender = null)
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

        /// <summary>
        /// <para>Прекратить владение объектом.</para>
        /// </summary>
        /// <param name="sender">игрок, запрашивающий владение объектом. Передается по умолчанию в сети mirror (не нужно указывать при вызове метода)</param>
        [Command(requiresAuthority = false)]
        public void CmdStopOwnership(NetworkConnectionToClient sender = null)
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

        /// <summary>
        /// <para>Продлить владение объектом.</para>
        /// Обнулить таймер владения объектом.
        /// Таким образом, текущий владелец объекта продолжит владеть объектом
        /// </summary>
        public void ExtendOwnership()
        {
            if (!AmIOwner())
            {
                return;
            }

            ownershipTimer = _ownershipTime;
        }

        /// <summary>
        /// <para>Имеет ли объект владельца.</para>
        /// </summary>
        /// <returns>true, если объект имеет владельца</returns>
        public bool HasOwner()
        {
            return hasOwner;
        }

        /// <summary>
        /// <para>Объект не имеет владельца.</para>
        /// </summary>
        /// <returns>true, если объект не имеет владельца</returns>
        public bool ObjectHasNoOwner()
        {
            return !hasOwner;
        }

        /// <summary>
        /// <para>Владеет ли объектом другой игрок.</para>
        /// </summary>
        /// <returns>true, если объектом владеет другой игрок</returns>
        public bool DoesObjectHaveAnotherOwner()
        {
            return hasOwner && !AmILastOwner();
        }

        /// <summary>
        /// <para>Владею ли я объектом.</para>
        /// </summary>
        /// <returns>true, если я владею объектом</returns>
        public bool AmIOwner()
        {
            return hasOwner && AmILastOwner();
        }

        /// <summary>
        /// <para>Являюсь ли я последним, кто владел данным объектом.</para>
        /// </summary>
        /// <returns>true, если после меня объектом никто не владел</returns>
        public bool AmILastOwner()
        {
            return myConnectionId != null && ownerId == myConnectionId;
        }

        /// <summary>
        /// <para>Является ли указанный пользователь последним, кто владел данным объектом.</para>
        /// </summary>
        /// <param name="connectionId">connection id пользователя</param>
        /// <returns>true, если указанный пользователь последним, кто владел данным объектом</returns>
        public bool IsLastOwnerConnectionId(int connectionId)
        {
            return connectionId == ownerId;
        }
    }
}