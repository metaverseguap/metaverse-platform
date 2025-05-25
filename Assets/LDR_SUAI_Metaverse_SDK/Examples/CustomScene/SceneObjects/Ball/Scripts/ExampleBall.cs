using LDR.SUAI_Metaverse.SDK.Core.Player;
using UnityEngine;

namespace CustomScene.SceneObjects.Ball
{
    /// <summary>
    /// <para>Пример мячика, который может пинать игрок.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ExampleBall : MonoBehaviour
    {
        [Tooltip("Сила пинка по мячу")]
        [SerializeField] private float _bounceForce = 0.5f;
        [Tooltip("Радиус поиска игрока вокруг мяча")]
        [SerializeField] private float _detectionRadius = 0.6f;
        [SerializeField] private LayerMask _playerLayer;

        private Rigidbody targetBody;

        private void Awake()
        {
            targetBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer);
            foreach (var hit in hits)
            {
                AbstractPlayer player = hit.GetComponent<AbstractPlayer>();
                if (player != null)
                {
                    Vector3 direction = (transform.position - hit.transform.position).normalized;

                    targetBody.AddForce(direction * _bounceForce, ForceMode.Impulse);
                }
            }
        }
    }
}