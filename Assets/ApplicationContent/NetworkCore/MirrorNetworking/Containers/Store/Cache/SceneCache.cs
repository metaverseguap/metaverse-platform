using System;
using System.Collections.Generic;
using NetworkCore.MirrorNetworking.Containers.Synchronization;
using NetworkCore.MirrorNetworking.Synchronization.UI;

namespace NetworkCore.MirrorNetworking.Containers.Store.Cache
{
    /// <summary>
    /// <para>Кеш сцены.</para>
    ///
    /// Данный класс необходим для временного хранения состояния сцены при миграции хоста
    /// </summary>
    public sealed class SceneCache
    {
        /// <summary>
        /// <para>Кеш положения синхронизируемых объектов.</para>
        /// Ключом является sceneId объекта, имеющего компоненты синхронизации.
        /// Значением являются <see cref="NamedTransform">координаты объекта в пространстве</see>
        /// </summary>
        public Dictionary<ulong, NamedTransform> SyncTransformsCache { get; } = new Dictionary<ulong, NamedTransform>();

        /// <summary>
        /// <para>Кеш содержимого UI текстов.</para>
        /// Ключом является sceneId объекта, имеющего компоненты синхронизации.
        /// Значением является массив текстов из <see cref="MVNetworkUI">объекта UI синхронизации</see>
        /// </summary>
        public Dictionary<ulong, string[]> SyncUITextsCache { get; } = new Dictionary<ulong, string[]>();

        /// <summary>
        /// <para>Кеш переменных аниматора.</para>
        /// Ключом является sceneId объекта, имеющего компоненты синхронизации.
        /// Значением является словарь переменных аниматора.
        /// В словаре переменных аниматора, ключом является имя переменной,
        /// а значением - значение этой переменной
        /// </summary>
        public Dictionary<ulong, Dictionary<string, object>> SyncAnimationVariableCache { get; set; } = new Dictionary<ulong, Dictionary<string, object>>();

        /// <summary>
        /// <para>Кеш id внешних анимаций, воспроизводимых в данный момент.</para>
        /// Ключом является sceneId объекта, имеющего компоненты синхронизации.
        /// Значением является пара из имени плейсхолдера внешних анимаций и списка id внешних анимаций, воспроизводимых в данный момент
        /// </summary>
        public Dictionary<ulong, Tuple<string, List<string>>> SyncExternalAnimationIdsCache { get; } = new Dictionary<ulong, Tuple<string, List<string>>>();

        /// <summary>
        /// <para>Очистить кеш.</para>
        /// </summary>
        public void Clear()
        {
            SyncTransformsCache.Clear();
            SyncUITextsCache.Clear();
            SyncAnimationVariableCache.Clear();
            SyncExternalAnimationIdsCache.Clear();
        }
    }
}