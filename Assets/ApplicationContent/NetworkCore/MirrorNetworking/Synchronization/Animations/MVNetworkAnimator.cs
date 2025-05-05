using System;
using System.Collections.Generic;
using System.Linq;
using Global.Logger;
using LDR.SUAI_Metaverse.SDK.Animations;
using Mirror;
using NetworkCore.MirrorNetworking.Containers.Store;
using UnityEngine;

namespace NetworkCore.MirrorNetworking.Synchronization.Animations
{
    /// <summary>
    /// <para>Компонент, синхронизирующий анимацию с сервером.</para>
    /// </summary>
    [DisallowMultipleComponent]
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
        
        private Dictionary<string, object> currentVariables = new Dictionary<string, object>();
        private Tuple<string, List<string>> externalAnimationsIds;

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

        /// <summary>
        /// Текущие значения переменных аниматора.
        /// </summary>
        public Dictionary<string, object> CurrentVariables => currentVariables;

        /// <summary>
        /// <para>Id внешних анимаций, воспроизводимых в данный момент.</para>
        /// Ключом является имя плейсхолдера внешних анимаций.
        /// Значением является список id внешних анимаций, воспроизводимых в данный момент
        /// </summary>
        public Tuple<string, List<string>> ExternalAnimationsIds => externalAnimationsIds;

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

            if (isServer)
            {
                NetworkDataStore store = MVNetworkManager.singleton.NetworkStore;
                CacheStore caches = store.HostMigration.Caches;
                if (caches.CurrentSceneCache.SyncAnimationVariableCache.TryGetValue(netIdentity.sceneId, out var cachedVariables))
                {
                    foreach (var variable in cachedVariables)
                    {
                        SetParameterValue(variable.Key, variable.Value);
                    }
                }

                if (caches.CurrentSceneCache.SyncExternalAnimationIdsCache.TryGetValue(netIdentity.sceneId, out var cachedExternalAnimations))
                {
                    if (cachedExternalAnimations.Item2.Count > 0)
                    {
                        CmdPlayExternalAnimation(cachedExternalAnimations.Item1, cachedExternalAnimations.Item2);
                    }
                }
            }
            else
            {
                CmdSetup();
            }
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
            if (animator == null)
            {
                return;
            }
            
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
        private void CmdSetup(NetworkConnectionToClient sender = null)
        {
            if (sender == null)
            {
                return;
            }
            
            foreach (var variable in currentVariables)
            {
                SetParameterValueOnlyForTarget(sender, variable);
            }

            if (externalAnimationsIds != null && externalAnimationsIds.Item2.Count > 0)
            {
                TargetPlayExternalAnimation(sender, externalAnimationsIds.Item1, externalAnimationsIds.Item2);
            }
        }

        private void SetParameterValueOnlyForTarget(NetworkConnectionToClient sender, KeyValuePair<string, object> variable)
        {
            var paramName = variable.Key;
            var value = variable.Value;
            var param = animator.parameters.FirstOrDefault(p => p.name == paramName);
            if (param == null)
            {
                AppLogger.Warning($"Animator parameter '{paramName}' not found.");
                return;
            }
                
            SetValueOnlyForTarget(sender, param, value);
        }

        private void SetValueOnlyForTarget(NetworkConnectionToClient sender, AnimatorControllerParameter param, object value)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    TargetSetAnimatorParameter(sender, param.name, Convert.ToBoolean(value));
                    break;
                case AnimatorControllerParameterType.Float:
                    TargetSetAnimatorParameter(sender, param.name, Convert.ToSingle(value));
                    break;
                case AnimatorControllerParameterType.Int:
                    TargetSetAnimatorParameter(sender, param.name, Convert.ToInt32(value));
                    break;
                case AnimatorControllerParameterType.Trigger:
                    break;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAnimatorParameter(string paramName, bool value)
        {
            RpcSetAnimatorParameter(paramName, value);
            animator.SetBool(paramName, value);
            currentVariables[paramName] = value;
        }

        [ClientRpc]
        private void RpcSetAnimatorParameter(string paramName, bool value)
        {
            if (!isServer && animator != null)
            {
                animator.SetBool(paramName, value);
                currentVariables[paramName] = value;
            }
        }
        
        // Этот метод будет вызван только на целевом клиенте
        [TargetRpc]
        private void TargetSetAnimatorParameter(NetworkConnection target, string paramName, bool value)
        {
            if (!isServer && animator != null)
            {
                animator.SetBool(paramName, value);
                currentVariables[paramName] = value;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAnimatorParameter(string paramName, float value)
        {
            RpcSetAnimatorParameter(paramName, value);
            animator.SetFloat(paramName, value);
            currentVariables[paramName] = value;
        }

        [ClientRpc]
        private void RpcSetAnimatorParameter(string paramName, float value)
        {
            if (!isServer && animator != null)
            {
                animator.SetFloat(paramName, value);
                currentVariables[paramName] = value;
            }
        }
        
        // Этот метод будет вызван только на целевом клиенте
        [TargetRpc]
        private void TargetSetAnimatorParameter(NetworkConnection target, string paramName, float value)
        {
            if (!isServer && animator != null)
            {
                animator.SetFloat(paramName, value);
                currentVariables[paramName] = value;
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAnimatorParameter(string paramName, int value)
        {
            RpcSetAnimatorParameter(paramName, value);
            animator.SetInteger(paramName, value);
            currentVariables[paramName] = value;
        }

        [ClientRpc]
        private void RpcSetAnimatorParameter(string paramName, int value)
        {
            if (!isServer && animator != null)
            {
                animator.SetInteger(paramName, value);
                currentVariables[paramName] = value;
            }
        }
        
        // Этот метод будет вызван только на целевом клиенте
        [TargetRpc]
        private void TargetSetAnimatorParameter(NetworkConnection target, string paramName, int value)
        {
            if (!isServer && animator != null)
            {
                animator.SetInteger(paramName, value);
                currentVariables[paramName] = value;
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
            if (!isServer && animator != null)
            {
                animator.SetTrigger(paramName);
            }
        }

        private void EndAnimation(Animator usedAnimator)
        {
            if (usedAnimator == animator)
            {
                animatingNow = false;
                externalAnimationsIds = null;
            }
        }

        private void PlayExternalAnimation(Animator usedAnimator, string externalAnimationPlaceholderName, List<AnimationClip> animationClips)
        {
            if (!HasAuthority() || usedAnimator != animator || animatingNow)
            {
                return;
            }

            animatingNow = true;

            externalAnimationsIds =
                new Tuple<string, List<string>>(
                    externalAnimationPlaceholderName,
                    animationClips
                        .Select(c => c.name)
                        .ToList()
                );
                

            CmdPlayExternalAnimation(externalAnimationsIds.Item1, externalAnimationsIds.Item2);
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
            externalAnimationsIds =
                new Tuple<string, List<string>>(
                    externalAnimationPlaceholderName,
                    clipsIds
                );
            RpcPlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
            PlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
        }

        [ClientRpc]
        private void RpcPlayExternalAnimation(string externalAnimationPlaceholderName, List<string> clipsIds)
        {
            if (!isServer && animator != null)
            {
                externalAnimationsIds =
                    new Tuple<string, List<string>>(
                        externalAnimationPlaceholderName,
                        clipsIds
                    );
                PlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
            }
        }
        
        // Этот метод будет вызван только на целевом клиенте
        [TargetRpc]
        private void TargetPlayExternalAnimation(NetworkConnection target, string externalAnimationPlaceholderName, List<string> clipsIds)
        {
            if (!isServer && animator != null)
            {
                externalAnimationsIds =
                    new Tuple<string, List<string>>(
                        externalAnimationPlaceholderName,
                        clipsIds
                    );
                PlayExternalAnimation(externalAnimationPlaceholderName, clipsIds);
            }
        }
    }
}