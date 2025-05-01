using System;
using System.Collections.Generic;
using System.Linq;
using Global.Logger;
using Mirror;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization.Animations
{
    /// <summary>
    /// <para>Компонент синхронизирующий анимацию с сервером.</para>
    /// </summary>
    public sealed class MVNetworkAnimator : NetworkBehaviour
    {
        [SerializeField] private AnimatorParameterListener parameterListener;
        [Tooltip("Если данный флаг поднят, то активировать анимацию может только локальный игрок, иначе любой игрок в комнате")]
        [SerializeField] private bool _clientAuthority;
        
        private Animator animator;
        
        [SyncVar]
        private bool animatingNow = false;
        [SyncVar]
        private bool clientAuthority = false;

        /// <summary>
        /// <see cref="AnimatorParameterListener"/>
        /// </summary>
        public AnimatorParameterListener ParameterListener
        {
            get => parameterListener;
            set
            {
                parameterListener = value;
                animator = parameterListener.ListenedAnimator;
                parameterListener.OnParameterChanged += AnimatorParameterChanged;
            }
        }

        /// <summary>
        /// Если данный флаг поднят, то активировать анимацию может только клиент.
        /// Иначе анимацию может активировать любой игрок на сервере.
        /// </summary>
        public bool ClientAuthority
        {
            get => clientAuthority;
            set
            {
                clientAuthority = value;
                _clientAuthority = clientAuthority;
            }
        }

        private void Start()
        {
            if (parameterListener != null)
            {
                animator = parameterListener.ListenedAnimator;
                parameterListener.OnParameterChanged += AnimatorParameterChanged;
            }

            clientAuthority = _clientAuthority;

            AnimationUtils.OnExternalAnimationStarted.AddListener(PlayExternalAnimation);
            AnimationUtils.OnExternalAnimationEnded.AddListener(EndAnimation);
        }

        private void OnDestroy()
        {
            if (parameterListener != null)
            {
                parameterListener.OnParameterChanged -= AnimatorParameterChanged;
            }

            AnimationUtils.OnExternalAnimationStarted.RemoveListener(PlayExternalAnimation);
            AnimationUtils.OnExternalAnimationEnded.RemoveListener(EndAnimation);
        }

        private void AnimatorParameterChanged(string parameterName, object parameterValue)
        {
            if (HasAuthority())
            {
                SetParameterValue(parameterName, parameterValue);
            }
        }
        
        private bool HasAuthority()
        {
            return isClient && isLocalPlayer || !clientAuthority;
        }

        private void SetParameterValue(string paramName, object value)
        {
            var param = animator.parameters.FirstOrDefault(p => p.name == paramName);
            if (param == null)
            {
                AppLogger.Warning($"Animator parameter '{paramName}' not found.");
                return;
            }

            SetValue(param, value);
        }

        private void SetValue(AnimatorControllerParameter param, object value)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    CmdSetAnimatorParameter(param.name, Convert.ToBoolean(value));
                    break;
                case AnimatorControllerParameterType.Float:
                    CmdSetAnimatorParameter(param.name, Convert.ToSingle(value));
                    break;
                case AnimatorControllerParameterType.Int:
                    CmdSetAnimatorParameter(param.name, Convert.ToInt32(value));
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if (Convert.ToBoolean(value))
                    {
                        CmdSetAnimatorSetTrigger(param.name);
                    }

                    break;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAnimatorParameter(string paramName, bool value)
        {
            RpcSetAnimatorParameter(paramName, value);
            animator.SetBool(paramName, value);
        }

        [ClientRpc]
        private void RpcSetAnimatorParameter(string paramName, bool value)
        {
            if (!isServer)
            {
                animator.SetBool(paramName, value);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAnimatorParameter(string paramName, float value)
        {
            RpcSetAnimatorParameter(paramName, value);
            animator.SetFloat(paramName, value);
        }

        [ClientRpc]
        private void RpcSetAnimatorParameter(string paramName, float value)
        {
            if (!isServer)
            {
                animator.SetFloat(paramName, value);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAnimatorParameter(string paramName, int value)
        {
            RpcSetAnimatorParameter(paramName, value);
            animator.SetInteger(paramName, value);
        }

        [ClientRpc]
        private void RpcSetAnimatorParameter(string paramName, int value)
        {
            if (!isServer)
            {
                animator.SetInteger(paramName, value);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAnimatorSetTrigger(string paramName)
        {
            RpcSetAnimatorSetTrigger(paramName);
            animator.SetTrigger(paramName);
        }

        [ClientRpc]
        private void RpcSetAnimatorSetTrigger(string paramName)
        {
            if (!isServer)
            {
                animator.SetTrigger(paramName);
            }
        }

        private void EndAnimation(Animator usedAnimator)
        {
            if (usedAnimator == animator)
            {
                animatingNow = false;
            }
        }

        private void PlayExternalAnimation(Animator usedAnimator, string externalAnimationPlaceholderName, List<AnimationClip> animationClips)
        {
            if (!HasAuthority() || usedAnimator != animator || animatingNow)
            {
                return;
            }

            animatingNow = true;

            List<string> clipsIds =
                animationClips
                    .Select(c => c.name)
                    .ToList();

            CmdPlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
        }

        private void PlayExternalAnimation(string externalAnimationPlaceholderName, List<string> clipsIds)
        {
            List<AnimationClip> animationClips =
                clipsIds
                    .Select(NetworkAnimationCache.GetAnimation)
                    .Where(clip => clip != null)
                    .ToList();

            StartCoroutine(AnimationUtils.PlayExternalAnimation(animator, externalAnimationPlaceholderName, animationClips));
        }

        [Command(requiresAuthority = false)]
        private void CmdPlayExternalAnimation(string externalAnimationPlaceholderName, List<string> clipsIds)
        {
            RpcPlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
            PlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
        }

        [ClientRpc]
        private void RpcPlayExternalAnimation(string externalAnimationPlaceholderName, List<string> clipsIds)
        {
            if (!isServer)
            {
                PlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
            }
        }
    }
}