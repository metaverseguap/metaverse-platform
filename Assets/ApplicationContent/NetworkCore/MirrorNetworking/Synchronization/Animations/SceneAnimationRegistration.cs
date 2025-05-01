using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkCore.MirrorNetworking.Synchronization.Animations
{
    /// <summary>
    /// <para>Компонент регистрирующий анимации внутри сцен.</para>
    /// </summary>
    public sealed class SceneAnimationRegistration : MonoBehaviour
    {
        #region Singleton

        private static SceneAnimationRegistration instance = null;

        private void Awake()
        {
            if (!InitSingleton()) return;
        }

        private bool InitSingleton()
        {
            if (instance != null && instance == this)
                return true;

            if (instance != null)
            {
                Destroy(gameObject);

                return false;
            }

            instance = this;
            if (Application.isPlaying)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            return true;
        }

        #endregion

        private ISet<string> scannedScenes = new HashSet<string>();
        private MVNetworkManager networkManager;

        private void Start()
        {
            networkManager = MVNetworkManager.singleton;

            networkManager.AfterServerChangeScene += AfterSceneLoaded;
            networkManager.AfterClientChangeScene += AfterSceneLoaded;
        }


        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.AfterServerChangeScene -= AfterSceneLoaded;
                networkManager.AfterClientChangeScene -= AfterSceneLoaded;
            }
        }

        private void AfterSceneLoaded(string sceneName)
        {
            AfterSceneLoaded();
        }

        private void AfterSceneLoaded()
        {
            StartCoroutine(CacheSceneAnimation());
        }

        private IEnumerator CacheSceneAnimation()
        {
            // Ждем один кадр, что бы сцена загрузилась
            yield return null;

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (scannedScenes.Add(currentSceneName))
            {
                CacheAnimatorsAnimations();
                CacheClipsFromSceneDeepScan();
            }
        }

        private static void CacheAnimatorsAnimations()
        {
            var animators = FindObjectsOfType<Animator>();

            foreach (var animator in animators)
            {
                var controller = animator.runtimeAnimatorController;
                if (controller == null) continue;

                foreach (var clip in controller.animationClips)
                {
                    NetworkAnimationCache.AddAnimation(clip);
                }
            }
        }

        private static void CacheClipsFromSceneDeepScan()
        {
            foreach (var sceneObject in FindObjectsOfType<GameObject>())
            {
                foreach (var component in sceneObject.GetComponents<MonoBehaviour>())
                {
                    if (component == null)
                    {
                        continue;
                    }

                    CacheComponentAnimations(component);
                }
            }
        }

        private static void CacheComponentAnimations(MonoBehaviour component)
        {
            var type = component.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(AnimationClip))
                {
                    if (field.GetValue(component) is AnimationClip clip)
                    {
                        NetworkAnimationCache.AddAnimation(clip);
                    }
                }
                else
                {
                    if (typeof(IEnumerable<AnimationClip>).IsAssignableFrom(field.FieldType))
                    {
                        if (field.GetValue(component) is IEnumerable<AnimationClip> enumerable)
                        {
                            foreach (var clip in enumerable)
                            {
                                if (clip != null)
                                {
                                    NetworkAnimationCache.AddAnimation(clip);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}