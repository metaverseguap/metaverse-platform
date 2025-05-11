using Mirror;

namespace NetworkCore.MirrorNetworking.ClientMessages
{
    /// <summary>
    /// <para>Сообщение, передающее клиенту данные о новом хосте для миграции.</para>
    /// </summary>
    public struct BackupHostMessage : NetworkMessage
    {
        /// <summary>
        /// Логин пользователя, который станет хостом, при отключении текущего хоста.
        /// </summary>
        public readonly string BackupHostLogin;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="backupHostLogin">логин пользователя, который станет хостом, при отключении текущего хоста</param>
        public BackupHostMessage(string backupHostLogin)
        {
            BackupHostLogin = backupHostLogin;
        }
    }
}