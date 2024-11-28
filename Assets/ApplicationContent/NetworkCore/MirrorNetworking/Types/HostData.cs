namespace NetworkCore.MirrorNetworking.Types
{
    /// <summary>
    /// <para>Данные хоста.</para>
    /// </summary>
    public class HostState
    {
        /// <summary>
        /// IP.
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Port.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Net ID.
        /// </summary>
        public uint NetID { get; set; }
        /// <summary>
        /// Название сцены, для которой он является хостом.
        /// </summary>
        public string SceneName { get; set; }
    }
}