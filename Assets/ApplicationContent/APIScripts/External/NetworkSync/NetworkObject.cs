using UnityEngine;

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

    /// <summary>
    /// Добавлять ли на объект NetworkIdentity.
    /// </summary>
    public bool SyncObject => _syncObject;

    /// <summary>
    /// Добавлять ли на объект NetworkAnimator.
    /// </summary>
    public bool AnimatedObject => _animatedObject;

    /// <summary>
    /// Добавлять ли на объект NetworkTransform.
    /// </summary>
    public bool TransformObject => _transformObject;

    /// <summary>
    /// Добавлять ли на объект NetworkRigidbody.
    /// </summary>
    public bool PhysicObject => _physicObject;

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
    }
}