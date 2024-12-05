using System.Collections.Generic;
using MainMenu.Containers;

namespace NetworkCore.MirrorNetworking.Containers
{
    /// <summary>
    /// <para>Хранилище сцен.</para>
    /// </summary>
    public sealed class SceneStore
    {
        public IList<SceneInfo> SceneInfos { get; set; } = new List<SceneInfo>();
    }
}