using System.Collections.Generic;
using MainMenu.Containers;

namespace NetworkCore.MirrorNetworking.Containers
{
    /// <summary>
    /// <para>Хранилище сцен.</para>
    /// </summary>
    public sealed class SceneStore
    {
        /// <summary>
        /// <para>Список всех сцен в приложении.</para>
        /// </summary>
        public IList<SceneInfo> SceneInfos { get; set; } = new List<SceneInfo>();

        /// <summary>
        /// <para>Сцена, в которой находится пользователь.</para>
        /// </summary>
        public SceneInfo CurrentScene { get; set; }
        
        /// <summary>
        /// <para>Имя сцены главного меню.</para>
        ///
        /// Данное имя нужно, для того что бы возвращаться в эту сцену после отключения от сети.
        /// </summary>
        public string MenuSceneName { get; set; }
    }
}