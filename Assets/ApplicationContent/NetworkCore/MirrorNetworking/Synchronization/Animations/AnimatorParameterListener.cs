using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NetworkCore.MirrorNetworking.Synchronization.Animations
{
    /// <summary>
    /// <para>Компонент отслеживающий состояние параметров аниматора.</para>
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class AnimatorParameterListener: MonoBehaviour
    {
        /// <summary>
        /// Событие вызываемое при изменении значения параметра аниматора.
        /// </summary>
        public event UnityAction<string, object> OnParameterChanged;
        
        private Animator animator;
        private Dictionary<string, object> parameterValues = new();

        /// <summary>
        /// Прослушиваемый аниматор.
        /// </summary>
        public Animator ListenedAnimator => animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            InitValues();
        }

        private void Update()
        {
            foreach (var param in animator.parameters)
            {
                string paramName = param.name;
                object currentValue = GetValue(param);

                if (!parameterValues.ContainsKey(paramName) || !Equals(parameterValues[paramName], currentValue))
                {
                    parameterValues[paramName] = currentValue;
                    OnParameterChanged?.Invoke(paramName, currentValue);
                }
            }
        }

        private void InitValues()
        {
            parameterValues.Clear();
            foreach (var param in animator.parameters)
            {
                parameterValues[param.name] = GetValue(param);
            }
        }

        private object GetValue(AnimatorControllerParameter param)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    return animator.GetFloat(param.name);
                case AnimatorControllerParameterType.Int:
                    return animator.GetInteger(param.name);
                case AnimatorControllerParameterType.Bool:
                case AnimatorControllerParameterType.Trigger:
                    return animator.GetBool(param.name);
                default:
                    return null;
            }
        }
    }
}