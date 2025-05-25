using System;

namespace LDR.SUAI_Metaverse.SDK.Core.NetworkSync.Support
{
    /// <summary>
    /// <para>Интерфейс объекта, предоставляющего доступ к сетевым функциям.</para>
    /// </summary>
    public interface INetworkEnvironmentProvider
    {
        /// <summary>
        /// Событие вызываемое при подключении к сетевой комнате.
        /// </summary>
        event Action OnConnectedToRoom;
        
        /// <summary>
        /// <para>Получить имя пользователя авторизованного в данный момент.</para>
        /// </summary>
        /// <returns>имя пользователя авторизованного в данный момент</returns>
        string GetUsername();

        /// <summary>
        /// <para>Подключен ли игрок к сетевой комнате в данный момент.</para>
        /// Сцена загружается раньше, чем игрок подключается к сетевой комнате.
        /// Что бы использовать методы сетевой комнаты, необходимо сначала проверить наличие подключения.
        /// Это нужно, в первую очередь, для методов <c>Start</c>, <c>Awake</c>, <c>OnEnable</c>
        /// </summary>
        /// <returns>true, если игрок уже подключен к сетевой комнате</returns>
        bool IsConnectedToRoom();
        
        /// <summary>
        /// <para>Получить время прошедшее с создания текущей комнаты.</para>
        /// <remarks>перед вызовом метода необходимо проверить есть ли подключение к сетевой комнате,
        /// при помощи метода <c>INetworkEnvironmentProvider.IsConnectedToRoom</c></remarks>
        /// </summary>
        /// <returns>время прошедшее с создания текущей комнаты</returns>
        TimeSpan GetRoomLifetime();
    }
}