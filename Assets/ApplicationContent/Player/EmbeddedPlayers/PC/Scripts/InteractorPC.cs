using System.Collections.Generic;
using UnityEngine;

namespace Player.EmbeddedPlayers.PC
{
    /// <summary>
    /// <para>Компонент отвечающий за взаимодействие игрока с интерактивными объектами при помощи клавиатуры и мышки.</para>
    /// </summary>
    public sealed class InteractorPC : MonoBehaviour
    {
        [Header("Key")]
        [SerializeField] private KeyCode _interactKey = KeyCode.F;

        [Header("Owner")]
        [Tooltip("От чьего имени осуществляется взаимодействие")]
        [SerializeField] private AbstractPlayer _owner;

        [Header("Setups")]
        [Tooltip("Точка из которой буде запущен луч взаимодействия по оси Z")]
        [SerializeField] private Transform _direction;

        [Tooltip("Максимальное расстояние, на котором будет работать взаимодействие с объектом")]
        [Range(0.1f, 10f)]
        [SerializeField]
        private float _range = 1f;

        private ISet<ITooltip> lastTooltip = new HashSet<ITooltip>();
        private GameObject lastTooltipObject = null;
        private GameObject lastInteractableObject = null;
        private IInteractable[] currentInteractables = null;
        private bool isKeyDown = false;

        private void FixedUpdate()
        {
            if (_owner != null && !_owner.PlayerController.ActiveController)
            {
                return;
            }

            CastInteractRay();
        }

        private void CastInteractRay()
        {
            Ray ray = new Ray(_direction.position, _direction.forward);

            bool hasHit = Physics.Raycast(ray, out RaycastHit hitInfo, _range);

            DrawDebugRay(hasHit, hitInfo, ray);

            if (hasHit)
            {
                GameObject hitObject = hitInfo.collider.gameObject;

                if (Input.GetKey(_interactKey))
                {
                    if (!isKeyDown)
                    {
                        isKeyDown = true;
                        InteractWithHitObject(hitObject);
                    }
                    else
                    {
                        HoldInteractWithHitObject(hitObject);
                    }
                }
                else
                {
                    isKeyDown = false;
                    lastInteractableObject = null;
                    if (currentInteractables != null)
                    {
                        foreach (IInteractable interactable in currentInteractables)
                        {
                            interactable.StopInteraction();
                        }
                    }
                    currentInteractables = null;
                }
                
                ShowHitObjectTooltip(hitObject);
            }
            else
            {
                ClearLastTooltip();
            }
        }

        private void InteractWithHitObject(GameObject hitObject)
        {
            IInteractable[] interactComponents = hitObject.GetComponents<IInteractable>();
            if (interactComponents != null && interactComponents.Length > 0)
            {
                lastInteractableObject = hitObject;
            }
            else
            {
                lastInteractableObject = null;
                return;
            }

            currentInteractables = interactComponents;
            
            foreach (IInteractable interactable in currentInteractables)
            {
                interactable.Interact(_owner.AvatarComponent.SpawnedAvatar.gameObject);
            }
        }
        
        private void HoldInteractWithHitObject(GameObject hitObject)
        {
            if (lastInteractableObject == null || currentInteractables == null || lastInteractableObject != hitObject)
            {
                return;
            }
            
            foreach (IInteractable interactable in currentInteractables)
            {
                if (interactable.AllowHoldInteraction)
                {
                    interactable.Interact(_owner.AvatarComponent.SpawnedAvatar.gameObject);
                }
            }
        }

        private void ShowHitObjectTooltip(GameObject hitObject)
        {
            ITooltip[] tooltips = hitObject.GetComponents<ITooltip>();

            if (lastTooltipObject != hitObject)
            {
                ClearLastTooltip();
                if (tooltips != null && tooltips.Length > 0)
                {
                    lastTooltipObject = hitObject;
                }
            }

            foreach (ITooltip tooltip in tooltips)
            {
                if (lastTooltip.Add(tooltip))
                {
                    tooltip.ShowTooltip();
                }
            }
        }

        private void ClearLastTooltip()
        {
            if (lastTooltip.Count > 0)
            {
                foreach (ITooltip tooltip in lastTooltip)
                {
                    tooltip.HideTooltip();
                }

                lastTooltip.Clear();
            }

            lastTooltipObject = null;
        }

        private void DrawDebugRay(bool hasHit, RaycastHit hitInfo, Ray ray)
        {
#if UNITY_EDITOR
            Color debugColor = Color.red;
            if (hasHit)
            {
                debugColor = Color.green;
                GameObject hitObject = hitInfo.collider.gameObject;
                IInteractable[] interactableObject = hitObject.GetComponents<IInteractable>();
                if (interactableObject != null && interactableObject.Length > 0)
                {
                    if (hitObject != lastInteractableObject)
                    {
                        lastInteractableObject = hitObject;
                    }

                    debugColor = Color.magenta;
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * _range, debugColor, 0.1f);
#endif
        }
    }
}