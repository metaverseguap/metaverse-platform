using UnityEngine;

namespace Adam.SceneObjects.Button.ButtonExecution
{
    /// <summary>
    /// <para>Скрипт, двигающий объект влево/вправо до указанного предела.</para>
    /// <remarks>скрипт является примером использования механики и не предназначен для использования в готовом проекте</remarks>
    /// </summary>
    public sealed class ExampleMoveObject : MonoBehaviour
    {
        [SerializeField] private Transform _leftBorder;
        [SerializeField] private Transform _rightBorder;

        private float minZ;
        private float maxZ;

        private void Start()
        {
            // Безопасно определяем границы: minX — левее, maxX — правее
            if (_leftBorder != null && _rightBorder != null)
            {
                minZ = Mathf.Min(_leftBorder.position.z, _rightBorder.position.z);
                maxZ = Mathf.Max(_leftBorder.position.z, _rightBorder.position.z);
            }
        }

        /// <summary>
        /// <para>Сдвинуть объект влево на указанное количество юнитов.</para>
        /// </summary>
        /// <param name="units">количество юнитов, на которое необходимо сдвинуть объект</param>
        public void MoveLeft(float units)
        {
            float distance = Mathf.Abs(units);
            float targetZ = transform.position.z - distance;
            targetZ = Mathf.Max(targetZ, minZ);

            transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
        }

        /// <summary>
        /// <para>Сдвинуть объект вправо на указанное количество юнитов.</para>
        /// </summary>
        /// <param name="units">количество юнитов, на которое необходимо сдвинуть объект</param>
        public void MoveRight(float units)
        {
            float distance = Mathf.Abs(units);
            float targetZ = transform.position.z + distance;
            targetZ = Mathf.Min(targetZ, maxZ);

            transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
        }
    }
}