using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.SceneLogic.SpawnPoint
{
    /// <summary>
    /// <para>Компонент, объединяющий информацию о точках спавна сцены.</para>
    /// </summary>
    public sealed class SceneSpawner : MonoBehaviour
    {
        #region Singleton

        private static SceneSpawner instance = null;

        private void Awake()
        {
            if (!InitSingleton()) return;
        }

        private bool InitSingleton()
        {
            if (instance != null && instance == this)
                return true;

            if (instance != null)
            {
                Destroy(gameObject);

                return false;
            }

            instance = this;
            if (Application.isPlaying)
            {
                transform.SetParent(null);
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Получить Scene Spawner текущей сцены.
        /// </summary>
        public static SceneSpawner singleton
        {
            get
            {
                if (instance == null)
                {
                    GameObject spawnerObj = new GameObject("SceneSpawner");
                    instance = spawnerObj.AddComponent<SceneSpawner>();
                }
                
                return instance;
            }
        }

        /// <summary>
        /// <para>Точки спавна текущей сцены.</para>
        ///
        /// Ключем служит InstanceID точки спавна
        /// </summary>
        private IDictionary<int, SpawnPoint> spawnPoints = new Dictionary<int, SpawnPoint>();
        
        private int offlineSpawnPointId = 0;
        private bool isOfflineSpawnActive = true;

        /// <summary>
        /// Активен ли спавн игроков без сетевого менеджера.
        /// </summary>
        public bool IsOfflineSpawnActive
        {
            get => isOfflineSpawnActive;
            set => isOfflineSpawnActive = value;
        }

        /// <summary>
        /// <para>Добавить точку спавна в набор точек спавна сцены.</para>
        /// </summary>
        /// <param name="spawnPoint">точка спавна</param>
        public void AddSpawnPoint(SpawnPoint spawnPoint)
        {
            spawnPoints.Add(spawnPoint.gameObject.GetInstanceID(), spawnPoint);

            RecalculateOfflineSpawnPoint();
        }

        /// <summary>
        /// <para>Удалить точку спавна из набора точек спавна сцены.</para>
        /// </summary>
        /// <param name="spawnPoint">точка спавна</param>
        public void RemoveSpawnPoint(SpawnPoint spawnPoint)
        {
            int spawnPointID = spawnPoint.gameObject.GetInstanceID();

            spawnPoints.Remove(spawnPointID);
            if (spawnPointID == offlineSpawnPointId)
            {
                RecalculateOfflineSpawnPoint();
            }
        }

        private void RecalculateOfflineSpawnPoint()
        {
            if (spawnPoints.Count <= 0)
            {
                return;
            }

            offlineSpawnPointId = spawnPoints.Keys.ElementAt(Random.Range(0, spawnPoints.Keys.Count));
        }

        /// <summary>
        /// <para>Получить список всех точек спавна сцены.</para>
        /// </summary>
        /// <returns>точки спавна текущей сцены</returns>
        public SpawnPoint[] SpawnPoints()
        {
            return spawnPoints.Values.ToArray();
        }

        /// <summary>
        /// <para>Является ли точка спавна, точкой спавна в офлайн сцене.</para>
        /// </summary>
        /// <param name="spawnPoint">проверяемая точка спавна</param>
        /// <returns>true, если точка спавна является точкой спавна в офлайн сцене</returns>
        public bool IsOfflineSpawnPoint(SpawnPoint spawnPoint)
        {
            return spawnPoints.Count > 0
                   && IsOfflineSpawnActive
                   && spawnPoint.gameObject.GetInstanceID() == offlineSpawnPointId;
        }
    }
}