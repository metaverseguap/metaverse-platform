using Mirror;
using NetworkCore.MirrorNetworking.Containers.ManagerSetups;
using NetworkCore.MirrorNetworking.Containers.Store;
using NetworkCore.MirrorNetworking.Utils;
using NetworkCore.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace NetworkCore.MirrorNetworking
{
    /// <summary>
    /// <para>Класс идентифицирующий локальную машину в сети.</para>
    ///
    /// Данный класс служит для подключения локальной машины к сети mirror,
    /// либо для создания из локальной машины хоста в сети mirror.
    ///
    /// Данный класс так же идентифицирует локальную машину в сети mirror.
    /// <remarks>к данному классу стоит относиться как к контейнеру, предоставляющему <see cref="NetworkDataStore">даннанные</see> и сетевые события.
    /// В данном класс не должно располагаться тяжелой логики</remarks>
    /// </summary>
    [RequireComponent(typeof(NetworkManagerSetups))]
    public sealed class MVNetworkManager : NetworkManager
    {
        /// <summary>
        /// Событие происходящие после запуска сервера или хоста.
        /// </summary>
        public event UnityAction AfterStartServerOrHost;
        
        /// <summary>
        /// Событие происходящие после подключения клиента.
        /// </summary>
        public event UnityAction AfterStartClient;
        
        /// <summary>
        /// Событие происходящие после подключения игрока к серверу.
        /// </summary>
        public event UnityAction AfterClientConnected;

        /// <summary>
        /// Событие происходящие после подключения к серверу нового игрока.
        /// </summary>
        public event UnityAction<NetworkConnectionToClient> AfterServerAddPlayer;
        
        /// <summary>
        /// Событие происходящие перед отключением игрока от сервера.
        /// </summary>
        public event UnityAction<NetworkConnectionToClient> BeforeServerLostPlayer;
        
        /// <summary>
        /// Событие происходящие после отключения игрока от сервера.
        /// </summary>
        public event UnityAction<NetworkConnectionToClient> AfterServerLostPlayer;

        /// <summary>
        /// Событие происходящие после подключения игрока к сети как хоста.
        /// </summary>
        public event UnityAction AfterHostStarted;

        /// <summary>
        /// Событие происходящее перед отключением клиента от сервера.
        /// </summary>
        public event UnityAction BeforeClientDisconnected;

        /// <summary>
        /// Событие происходящие до того, как сервер изменил сцену.
        /// </summary>
        public event UnityAction<string> BeforeServerChangeScene;
        
        /// <summary>
        /// Событие происходящие после того, как сервер изменил сцену.
        /// </summary>
        public event UnityAction<string> AfterServerChangeScene;
        
        /// <summary>
        /// Событие происходящие перед остановкой сервера.
        /// </summary>
        public event UnityAction BeforeServerStop;

        /// <summary>
        /// Настройки менеджера, устанавливаемые через редактор Unity.
        /// </summary>
        private NetworkManagerSetups networkManagerSetups;

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.singleton"/></para>
        /// <para>Доступ к данному типу NetworkManager.</para>
        /// 
        /// <para>Данное свойство стоит использовать для получения экземпляра MVNetworkManager в других классах.</para>
        /// <para>Если в сцене нет NetworkManager, то данное свойство вернет null.</para>
        /// <para>По контракту, NetworkManager обязательно присутствует в пространствах имен NetworkCore.MirrorNetworking и MainMenu.
        /// В остальных местах перед вызовом данного метода следует проверить возможность его использования при помощи <see cref="MVNetworkManager.IsOnline"/></para>
        /// </summary>
        public new static MVNetworkManager singleton { get; private set; }

        /// <summary>
        /// <para>Есть ли в данный момент подключение к сети mirror.</para>
        ///
        /// Для взаимодействия с сетевыми функциями нужно вызвать статический метод <see cref="MVNetworkManager.singleton"/>
        /// и использовать результат его работы для взаимодействия с сетью. Но если сцена запускается автономно,
        /// то в ней нет MVNetworkManager и взаимодействие с сетью через <see cref="MVNetworkManager.singleton"/> невозможно.
        /// Данный метод проверяет, возможно ли в текущей сцене использовать сетевые функции. Если метод вернет true,
        /// значит можно, воспользовавшись методом <see cref="MVNetworkManager.singleton"/>, взаимодействовать с сетевыми функциями.
        /// Если данный метод возвращает false, то сетевые функции должны быть недоступны, но сами компоненты и скрипты должны продолжать функционировать.
        /// </summary>
        /// <returns>true, если в данный момент есть подключение к сети mirror</returns>
        public static bool IsOnline()
        {
            return singleton != null;
        }
        
        /// <summary>
        /// <para>Отсутствует ли в данный момент подключение к сети mirror.</para>
        ///
        /// Для взаимодействия с сетевыми функциями нужно вызвать статический метод <see cref="MVNetworkManager.singleton"/>
        /// и использовать результат его работы для взаимодействия с сетью. Но если сцена запускается автономно,
        /// то в ней нет MVNetworkManager и взаимодействие с сетью через <see cref="MVNetworkManager.singleton"/> невозможно.
        /// Данный метод проверяет, отсутствие возможности в текущей сцене использовать сетевые функции. Если метод вернет true,
        /// значит сетевые функции должны быть недоступны, но сами компоненты и скрипты должны продолжать функционировать.
        /// </summary>
        /// <returns>true, если в данный момент нет подключения к сети mirror</returns>
        public static bool IsOffline()
        {
            return !IsOnline();
        }

        /// <summary>
        /// Хранилище данных.
        /// </summary>
        public NetworkDataStore NetworkStore { get; private set; }

        public override void Awake()
        {
            base.Awake();
            singleton = NetworkManager.singleton as MVNetworkManager;
            networkManagerSetups = GetComponent<NetworkManagerSetups>();
            NetworkStore = new NetworkDataStore(networkManagerSetups);
            // Отключаем автоматическую загрузку сцены при отключении от сервера.
            // Загрузки сцены контролируются вручную в расширениях NetworkManagerExtensions
            offlineScene = null;
            onlineScene = networkManagerSetups.DefaultScene;
            maxConnections = networkManagerSetups.MaxConnections;
            networkAddress = IPUtils.GetIpAsUrl();
            
            // Регистрируем спавнемые в сцене префабы
            this.RegisterPrefab(NetworkStore.Player.NetworkPlayer.gameObject);
            this.RegisterPrefab(NetworkStore.Player.DisplayName.gameObject);
        }
        
        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnStartServer"/></para>
        ///
        /// <remarks>метод, вызываемый при запуске сервера или хоста</remarks>
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();
            AfterStartServerOrHost?.Invoke();
        }

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnStartHost"/></para>
        ///
        /// <remarks>метод, вызываемый при подключении машины как хоста</remarks>
        /// </summary>
        public override void OnStartHost()
        {
            base.OnStartHost();
            AfterHostStarted?.Invoke();
        }
        
        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnStartClient"/></para>
        ///
        /// <remarks>метод, вызываемый при запуске клиента</remarks>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            AfterStartClient?.Invoke();
        }

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnClientConnect"/></para>
        ///
        /// <remarks>метод, вызываемый при попытке игрока(я как клиент подключаюсь) подключиться к серверу</remarks>
        /// </summary>
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            AfterClientConnected?.Invoke();
        }

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnClientConnect"/></para>
        ///
        /// <remarks>метод, вызываемый при подключении нового игрока(ко мне как к серверу подключаются) к серверу</remarks>
        /// </summary>
        /// <param name="conn">сведенья о подключенном игроке</param>
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            AfterServerAddPlayer?.Invoke(conn);
        }

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.ServerChangeScene"/></para>
        ///
        /// <remarks>метод, вызываемый когда сервер меняет сцену</remarks>
        /// </summary>
        /// <param name="newSceneName">имя новой сцены</param>
        public override void ServerChangeScene(string newSceneName)
        {
            BeforeServerChangeScene?.Invoke(newSceneName);
            
            base.ServerChangeScene(newSceneName);
            AfterServerChangeScene?.Invoke(newSceneName);
        }

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnClientDisconnect"/></para>
        ///
        /// <remarks>метод, вызываемый при отключении игрока(я как клиент отключаюсь) от сервера</remarks>
        /// </summary>
        public override void OnClientDisconnect()
        {
            BeforeClientDisconnected?.Invoke();
            base.OnClientDisconnect();
        }

        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnServerDisconnect"/></para>
        ///
        /// <remarks>метод, вызываемый когда игрок(от меня как от сервера отключился какой-то игрок) покинул сервер</remarks>
        /// </summary>
        /// <param name="conn">сведенья об отключаемом игроке</param>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            BeforeServerLostPlayer?.Invoke(conn);
            base.OnServerDisconnect(conn);
            AfterServerLostPlayer?.Invoke(conn);
        }
        
        /// <summary>
        /// <para><inheritdoc cref="NetworkManager.OnStopServer"/></para>
        ///
        /// <remarks>метод, вызываемый при остановке сервера или хоста</remarks>
        /// </summary>
        public override void OnStopServer()
        {
            BeforeServerStop?.Invoke();
            base.OnStopServer();
        }
    }
}