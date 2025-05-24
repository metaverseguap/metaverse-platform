using UnityEngine;

namespace LDR.SUAI_Metaverse.SDK.Components.NetworkSync
{
    /// <summary>
    /// <para>Маркер сетевого объекта.</para>
    /// </summary>
    public sealed class NetworkObject : MonoBehaviour
    {
        [Tooltip("Объект считается сетевым объектом.")]
        [SerializeField] private bool _syncObject = true;

        [Tooltip("Объект должен воспроизводить анимации в сети.")]
        [SerializeField] private bool _animatedObject;

        [Tooltip("Объект должен изменять свое положение в сети.")]
        [SerializeField] private bool _transformObject;

        [Tooltip("Объект должен использовать RigidBody в сети.")]
        [SerializeField] private bool _physicObject;

        [Tooltip("Объект может иметь владельца. Пока объектом владеет один пользователь, остальные не могут с ним взаимодействовать.")]
        [SerializeField] private bool _canBeOwned;

        [Tooltip("Объект является UI canvas, содержимое которого нужно синхронизировать")]
        [SerializeField] private bool _canvas;

        /// <summary>
        /// Добавлять ли на объект сетевую идентификацию.
        /// </summary>
        public bool SyncObject => _syncObject;

        /// <summary>
        /// Добавлять ли на объект синхронизацию анимации.
        /// </summary>
        public bool AnimatedObject => _animatedObject;

        /// <summary>
        /// Добавлять ли на объект синхронизацию положения.
        /// </summary>
        public bool TransformObject => _transformObject;

        /// <summary>
        /// Добавлять ли на объект синхронизацию положения Rigidbody.
        /// </summary>
        public bool PhysicObject => _physicObject;

        /// <summary>
        /// Добавлять ли компонент синхронизации владельца объекта.
        /// </summary>
        public bool CanBeOwned => _canBeOwned;

        /// <summary>
        /// Нужно ли добавлять компонент синхронизации UI.
        /// </summary>
        public bool IsCanvas => _canvas;

        private void OnValidate()
        {
            // Костыль для readonly поля
            _syncObject = true;

            // Нельзя поставить checkbox, если на объекте нет аниматора
            if (_animatedObject)
            {
                if (GetComponent<Animator>() == null)
                {
                    _animatedObject = false;
                }
            }

            // Нельзя одновременно Transform и Rigidbody синхронизацию
            if (_transformObject)
            {
                _physicObject = false;
            }
            else
            {
                // Нельзя поставить checkbox, если на объекте нет Rigidbody
                if (GetComponent<Rigidbody>() == null)
                {
                    _physicObject = false;
                }
            }

            // Нельзя поставить checkbox, если на объекте нет canvas
            if (_canvas)
            {
                if (GetComponent<Canvas>() == null)
                {
                    _canvas = false;
                }
            }
        }
    }
}