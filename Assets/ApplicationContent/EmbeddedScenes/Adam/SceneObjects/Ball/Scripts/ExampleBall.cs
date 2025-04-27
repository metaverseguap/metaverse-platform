using Global.Logger;
using Player.EmbeddedPlayers;
using UnityEngine;

namespace Adam.SceneObjects.Ball
{
    /// <summary>
    /// <para>Пример мячика, который может пинать игрок.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ExampleBall : MonoBehaviour
    {
        [Tooltip("Сила пинка по мячу")]
        [SerializeField] private float bounceForce = 5f;
        [SerializeField] private float detectionRadius = 0.6f; // радиус поиска игрока вокруг мяча
        [SerializeField] private LayerMask playerLayer;

        private Rigidbody targetBody;

        private void Awake()
        {
            targetBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            foreach (var hit in hits)
            {
                AbstractPlayer player = hit.GetComponent<AbstractPlayer>();
                if (player != null)
                {
                    AppLogger.Log($"Kicked by {player.name}");
                    Vector3 direction = (transform.position - hit.transform.position).normalized;

                    targetBody.AddForce(direction * bounceForce, ForceMode.Impulse);
                }
            }
        }
    }
}