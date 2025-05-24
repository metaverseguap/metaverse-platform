using System;
using LDR.SUAI_Metaverse.SDK.Interactions;
using LDR.SUAI_Metaverse.SDK.NetworkSync.Support;
using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.NetworkSync
{
    /// <summary>
    /// <para>Компонент, предоставляющий сторонним разработчикам доступ к сетевым функциям.</para>
    /// <para>Данный компонент является заглушкой, позволяющей сторонним разработчикам создавать объекты синхронизирующие поведение с сетью,
    /// без непосредственного доступа к сети.</para>
    /// <para>В данном компоненте содержаться методы, позволяющие получать сетевую информацию, которую можно использовать для синхронизации локальной логики с сетью</para>
    /// </summary>
    public static class NetworkEnvironment
    {
        /// <summary>
        /// Объект предоставляющий доступ к сетевым функциям.
        /// </summary>
        public static INetworkEnvironmentProvider NetworkProvider { get; set; } = new DefaultNetworkProvider();

        /// <summary>
        /// <para>Получить имя пользователя авторизованного в данный момент.</para>
        /// </summary>
        /// <returns>имя пользователя авторизованного в данный момент</returns>
        public static string GetUsername()
        {
            return NetworkProvider.GetUsername();
        }

        /// <summary>
        /// <para>Метод вызывающий callback, когда игрок окончательно подключится к комнате</para>
        /// </summary>
        /// <param name="callback">callback</param>
        public static void WhenConnectToRoom(Action callback)
        {
            if (IsConnectedToRoom())
            {
                callback?.Invoke();
            }
            else
            {
                NetworkProvider.OnConnectedToRoom += callback;
            }
        }
        
        /// <summary>
        /// <para>Подключен ли игрок к сетевой комнате в данный момент.</para>
        /// Сцена загружается раньше, чем игрок подключается к сетевой комнате.
        /// Что бы использовать методы сетевой комнаты, необходимо сначала проверить наличие подключения.
        /// Это нужно, в первую очередь, для методов <c>Start</c>, <c>Awake</c>, <c>OnEnable</c>
        /// </summary>
        /// <returns>true, если игрок уже подключен к сетевой комнате</returns>
        public static bool IsConnectedToRoom()
        {
            return NetworkProvider.IsConnectedToRoom();
        }
        
        /// <summary>
        /// <para>Получить время прошедшее с создания текущей комнаты.</para>
        ///
        /// <remarks>из-за того, что подключение к комнате происходит после загрузки сцены,
        /// данный метод может возвращать некорректные значения в методах <c>Start</c>, <c>Awake</c>, <c>OnEnable</c>.
        /// Что бы этого избежать, данный метод стоит вызывать внутри callback-а <c>NetworkEnvironment.WhenConnectToRoom(() => currentTime = NetworkEnvironment.GetRoomLifetime());</c>,
        /// либо вручную проверять наличие подключения, используя метод <c>NetworkEnvironment.IsConnectedToRoom</c></remarks>
        /// </summary>
        /// <returns>время прошедшее с создания текущей комнаты</returns>
        public static TimeSpan GetRoomLifetime()
        {
            return NetworkProvider.GetRoomLifetime();
        }

        /// <summary>
        /// <para>Получить <see cref="IInteractionAccess"/> указанного объекта.</para>
        /// </summary>
        /// <param name="gameObject">объект</param>
        /// <returns><see cref="IInteractionAccess"/> данного объекта</returns>
        public static IInteractionAccess GetInteractionAccessor(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out IInteractionAccess externalAccessComponent))
            {
                return externalAccessComponent;
            }

            return gameObject.AddComponent<DefaultInteractionAccess>();
        }
    }
}