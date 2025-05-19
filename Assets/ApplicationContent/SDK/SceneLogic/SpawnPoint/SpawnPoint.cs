using LDR.SUAI_Metaverse.SDK.NetworkSync;
using LDR.SUAI_Metaverse.SDK.Player;
using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.SceneLogic.SpawnPoint
{
    /// <summary>
    /// <para>Точка спавна игрока в сцене.</para>
    /// </summary>
    public sealed class SpawnPoint : MonoBehaviour
    {
        [Tooltip("Префаб контроллера игрока, спавнемый в офлайн сцене")] 
        [SerializeField] private AbstractPlayer _offlinePlayer;

        private SceneSpawner sceneSpawner;

        private void Awake()
        {
            sceneSpawner = SceneSpawner.singleton;

            sceneSpawner.AddSpawnPoint(this);
        }

        private void Start()
        {
            NetworkEnvironment.WhenConnectToRoom(SpawnPlayer);
        }

        private void SpawnPlayer()
        {
            if (sceneSpawner.IsOfflineSpawnPoint(this))
            {
                Instantiate(_offlinePlayer, transform.position, transform.rotation);
            }
        }
    }
}