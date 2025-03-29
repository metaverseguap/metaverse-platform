using Mirror;
using Player.EmbeddedPlayers;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Player.Base
{
    /// <summary>
    /// <para>Базовый класс сетевого игрока.</para>
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkBasePlayer : NetworkBehaviour
    {
        private AbstractPlayerController playerController;

        // Переменная синхронизирована с сервером
        [SyncVar] private string displayName = "Loading...";

        /// <summary>
        /// Отображаемое имя игрока.
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// <para>Установить отображаемое имя игрока.</para>
        /// <remarks>данный метод выполняется на сервере.
        /// Это нужно, что бы локальная машина не затирала значения переменной других игроков своим локальным значением</remarks>
        /// </summary>
        /// <param name="displayName">отображаемое имя игрока</param>
        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        /// <summary>
        /// Ссылка на контроллер игрока.
        /// </summary>
        public AbstractPlayerController PlayerController { get; set; }

        private void Start()
        {
            OnStart();
        }

        /// <summary>
        /// <para>Метод выполняющийся в первый кадр присутствия объекта в сцене.</para>
        /// </summary>
        protected virtual void OnStart()
        {
            RefreshControllerActivation();
        }

        /// <summary>
        /// <para>Обновить активацию контроллера данного сетевого игрока.</para>
        /// Если данный сетевой игрок относится к данной машине, то контроллер будет активирован и перехватит управление.
        /// Иначе контроллер будет деактивирован и не будет мешать управлению другими контроллерами.
        /// </summary>
        public void RefreshControllerActivation()
        {
            if (PlayerController != null)
            {
                bool isCurrentPlayerController = isClient && isLocalPlayer;
                AbstractPlayerController[] controllers = PlayerController.gameObject.GetComponentsInChildren<AbstractPlayerController>();
                if (isCurrentPlayerController)
                {
                    ActivateController(controllers);
                }
                else
                {
                    DeactivateController(controllers);
                }
            }
        }

        private void ActivateController(AbstractPlayerController[] controllers)
        {
            foreach (AbstractPlayerController controller in controllers)
            {
                controller.ActiveController = false;
            }
            PlayerController.ActiveController = true;
        }

        private void DeactivateController(AbstractPlayerController[] controllers)
        {
            PlayerController.DeactivatePermanently();
            foreach (AbstractPlayerController controller in controllers)
            {
                controller.DeactivatePermanently();
            }
        }
    }
}