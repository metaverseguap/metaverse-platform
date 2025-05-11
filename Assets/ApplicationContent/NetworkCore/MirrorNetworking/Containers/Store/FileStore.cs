namespace NetworkCore.MirrorNetworking.Containers.Store
{
    /// <summary>
    /// <para>Хранилище файлов с файлового сервера.</para>
    /// </summary>
    public sealed class FileStore
    {
        /// <summary>
        /// Хранилище аватаров.
        /// </summary>
        public AvatarStore Avatars { get; } = new AvatarStore();

        /// <summary>
        /// Хранилище сцен.
        /// </summary>
        public SceneStore Scenes { get; } = new SceneStore();
    }
}