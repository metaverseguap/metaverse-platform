using Global.Logger;
using kcp2k;
using MainMenu.Containers;
using Mirror;
using NetworkCore.MirrorNetworking.Containers;
using NetworkCore.ServerInteraction.API;
using NetworkCore.ServerInteraction.Type.Host;
using NetworkCore.Utils;
using OfflineScene;
using UnityEngine.SceneManagement;

namespace NetworkCore.MirrorNetworking.Utils
{
    /// <summary>
    /// <para>Методы расширений для класса <see cref="MVNetworkManager"/>.</para>
    /// </summary>
    public static class NetworkManagerExtensions
    {
        /// <summary>
        /// <para>Отключиться от сети Mirror.</para>
        /// </summary>
        /// <param name="connection"><see cref="MVNetworkManager"/></param>
        public static void DisconnectFromNetwork(this MVNetworkManager connection)
        {
            APIContainer serverAPI = connection.NetworkStore.FileServer;

            if (AmIHost())
            {
                connection.StopHost();
                serverAPI.Hosts.RemoveHost();
                return;
            }

            if (AmIClient())
            {
                connection.StopClient();
                return;
            }

            if (AmIServer())
            {
                connection.StopServer();
                serverAPI.Hosts.RemoveHost();
                return;
            }
        }

        private static bool AmIHost()
        {
            return AmIServer() && AmIClient();
        }

        private static bool AmIClient()
        {
            return NetworkClient.isConnected;
        }

        private static bool AmIServer()
        {
            return NetworkServer.active;
        }

        /// <summary>
        /// <para>Подключиться к сети в зависимости от текущего состояния store.</para>
        ///
        /// <para>Для работы данного метода в NetworkDataStore должен быть указан корректный <c>Player.PlayerStatus</c>.</para>
        /// <para>Если игрок подключается как хост <c>Player.PlayerStatus == Host</c>,
        /// то так же необходимо, что бы в <c>NetworkDataStore</c> была указана сцена подключения <c>Scenes.CurrentScene</c>.</para>
        /// <para>Если игрок подключается как клиент <c>Player.PlayerStatus == Client</c>,
        /// то так же необходимо, что бы в <c>MVNetworkManager.networkAddress</c> был указан url хоста.
        /// Для корректного отображения комнаты на UI, так же стоит указывать сцену подключения в <c>Scenes.CurrentScene</c>,
        /// несмотря на то что непосредственно для подключения клиента данный параметр не используется</para>
        /// </summary>
        /// <param name="connection"><see cref="MVNetworkManager"/></param>
        public static void ConnectToNetwork(this MVNetworkManager connection)
        {
            NetworkDataStore store = connection.NetworkStore;
            SceneInfo loadingScene = store.Scenes.CurrentScene;

            var connectionStatus = store.Player.PlayerStatus;
            if (connectionStatus == PlayerConnectionStatus.Host
                || loadingScene.Name == OfflineSceneConstants.SCENE_INFO.Name)
            {
                connection.StartHost();
                string sceneName = loadingScene.GetSceneLoadingName();
                connection.ServerChangeScene(sceneName);
                return;
            }

            if (connectionStatus == PlayerConnectionStatus.Client)
            {
                connection.StartClient();
                return;
            }

            AppLogger.Error($"Player has an incorrect connection status: {connectionStatus}");
            connection.StartOfflineScene();
        }

        /// <summary>
        /// <para>Перейти в офлайн сцену.</para>
        /// </summary>
        /// <param name="connection"><see cref="MVNetworkManager"/></param>
        public static void StartOfflineScene(this MVNetworkManager connection)
        {
            BecomeHost(connection, OfflineSceneConstants.SCENE_INFO);
        }

        /// <summary>
        /// <para>Стать хостом указанной сцены.</para>
        /// </summary>
        /// <param name="connection"><see cref="MVNetworkManager"/></param>
        /// <param name="scene"><see cref="SceneInfo">информация о сцене</see>, хостом которой становится пользователь</param>
        public static void BecomeHost(this MVNetworkManager connection, SceneInfo scene)
        {
            NetworkDataStore store = connection.NetworkStore;
            APIContainer serverAPI = store.FileServer;

            store.Scenes.CurrentScene = scene;
            store.Player.PlayerStatus = PlayerConnectionStatus.Host;

            string address;
            int port;
            if (scene.Name == OfflineSceneConstants.SCENE_INFO.Name)
            {
                address = IPUtils.GetIpAsUrl();
                int? availablePort = IPUtils.GetAvailableUDPPort(200);
                if (!availablePort.HasValue)
                {
                    AppLogger.Error("No available ports for new offline scene host");
                    
                    // Загружаем сцену меню
                    string menuScene = connection.NetworkStore.Scenes.MenuSceneName;
                    connection.NetworkStore.Scenes.CurrentScene = null;
                    SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
                    return;
                }
                
                port = availablePort.Value;
            }
            else
            {
                HostAddressDTO createdHostAddress = serverAPI.Hosts.BecomeAHost(scene.Name);
                address = createdHostAddress.hostIP;
                port = createdHostAddress.port;
            }

            connection.networkAddress = address;
            if (connection.transport is KcpTransport connectionTransport)
            {
                connectionTransport.port = (ushort)port;
            }
            
            // Загружаем сцену загрузки, из которой будет запущен хост
            string loadingSceneName = store.Scenes.LoadingSceneName;
            SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Single);
        }

        /// <summary>
        /// <para>Подключиться к хосту по Uri.</para>
        /// </summary>
        /// <param name="connection"><see cref="MVNetworkManager"/></param>
        /// <param name="hostIP">uri хоста</param>
        /// <param name="port">порт хоста</param>
        /// <param name="scene">сцена, в которой находится хост</param>
        public static void BecomeAClient(this MVNetworkManager connection, string hostIP, int port, SceneInfo scene)
        {
            NetworkDataStore store = connection.NetworkStore;
            store.Player.PlayerStatus = PlayerConnectionStatus.Client;
            store.Scenes.CurrentScene = scene;
            connection.networkAddress = hostIP;
            if (connection.transport is KcpTransport connectionTransport)
            {
                connectionTransport.port = (ushort)port;
            }
            
            // Загружаем сцену загрузки, из которой будет запущен клиент
            string loadingSceneName = store.Scenes.LoadingSceneName;
            SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Single);
        }
    }
}