using System;
using Global.Containers;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Containers.Synchronization
{
    /// <summary>
    /// <para>Координаты игрового объекта с именем объекта.</para>
    /// </summary>
    [Serializable]
    public sealed class NamedTransform : INamedContainer
    {
        /// <summary>
        /// Идентификатор объекта.
        /// </summary>
        public string GameObjectIdentifier;
        
        /// <summary>
        /// <inheritdoc cref="INamedContainer.Name"/>
        /// </summary>
        public string Name
        {
            get => GameObjectIdentifier;
            set => GameObjectIdentifier = value;
        }

        /// <summary>
        /// Положение объекта.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Поворот объекта.
        /// </summary>
        public Quaternion Rotation;

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        public NamedTransform() : this(null, Vector3.zero, Quaternion.identity)
        {
        }

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="gameObjectIdentifier">идетнификатор объекта</param>
        public NamedTransform(string gameObjectIdentifier) : this(gameObjectIdentifier, Vector3.zero, Quaternion.identity)
        {
        }

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="gameObjectIdentifier">идетнификатор объекта</param>
        /// <param name="position">положение объекта</param>
        public NamedTransform(string gameObjectIdentifier, Vector3 position) : this(gameObjectIdentifier, position, Quaternion.identity)
        {
        }

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="gameObjectIdentifier">идетнификатор объекта</param>
        /// <param name="rotation">поворот объекта</param>
        public NamedTransform(string gameObjectIdentifier, Quaternion rotation) : this(gameObjectIdentifier, Vector3.zero, rotation)
        {
        }

        /// <summary>
        /// <para>Конструктор.</para>
        /// </summary>
        /// <param name="gameObjectIdentifier">идетнификатор объекта</param>
        /// <param name="position">положение объекта</param>
        /// <param name="rotation">поворот объекта</param>
        public NamedTransform(string gameObjectIdentifier, Vector3 position, Quaternion rotation)
        {
            GameObjectIdentifier = gameObjectIdentifier;
            Position = position;
            Rotation = rotation;
        }
    }
}