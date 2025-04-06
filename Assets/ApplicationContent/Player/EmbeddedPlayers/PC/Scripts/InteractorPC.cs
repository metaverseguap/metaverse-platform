using UnityEngine;
using UnityEngine.Serialization;

namespace Player.EmbeddedPlayers.PC
{
    /// <summary>
    /// <para>Компонент отвечающий за взаимодействие игрока с интерактивными объектами при помощи клавиатуры и мышки.</para>
    /// </summary>
    public sealed class InteractorPC : MonoBehaviour
    {
        [Header("Key")] 
        [SerializeField] private KeyCode _interactKey = KeyCode.F;

        [Header("Owner")] [Tooltip("От чьего имени осуществляется взаимодействие")]
        [SerializeField] private AbstractPlayer _owner;
        
        [Header("Setups")]
        [Tooltip("Точка из которой буде запущен луч взаимодействия по оси Z")]
        [SerializeField] private Transform _direction;

        [Tooltip("Максимальное расстояние, на котором будет работать взаимодействие с объектом")]
        [Range(0.1f, 10f)]
        [SerializeField] private float _range = 1f;

        private ITooltip lastTooltip;

        private void Update()
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
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _range))
            {
                if (Input.GetKeyDown(_interactKey))
                {
                    InteractWithHitObject(hitInfo);
                }
                else
                {
                    ShowHitObjectTooltip(hitInfo);
                }
            }
            else
            {
                if (lastTooltip != null)
                {
                    lastTooltip.HideTooltip();
                    lastTooltip = null;
                }
            }
        }

        private void InteractWithHitObject(RaycastHit hitInfo)
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(_owner.AvatarComponent.SpawnedAvatar);
            }
        }

        private void ShowHitObjectTooltip(RaycastHit hitInfo)
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out ITooltip tooltip))
            {
                if (lastTooltip == null || tooltip != lastTooltip)
                {
                    lastTooltip = tooltip;
                }

                lastTooltip.ShowTooltip();
            }
        }
    }
}