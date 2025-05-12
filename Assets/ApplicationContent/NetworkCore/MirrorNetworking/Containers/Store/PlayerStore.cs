namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище данных об игроке.</para>
    /// </summary>
    public sealed class PlayerStore
    {
        /// <summary>
        /// Логин текущего игрока.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Имя аватара игрока текущего пользователя.
        /// </summary>
        public string AvatarName { get; set; }

        /// <summary>
        /// <para>Очистить контейнер.</para>
        /// </summary>
        public void Clear()
        {
            Login = null;
            AvatarName = null;
        }
    }
}