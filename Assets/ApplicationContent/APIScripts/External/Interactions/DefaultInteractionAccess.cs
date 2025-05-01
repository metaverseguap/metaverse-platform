using UnityEngine;

/// <summary>
/// <para>Компонент по умолчанию, проверяющий возможность взаимодействия с интерактивным объектом в данный момент.</para>
/// </summary>
public sealed class DefaultInteractionAccess : MonoBehaviour, IInteractionAccess
{
    [SerializeField] private bool _allowInteraction = true;

    private bool hasControl = false;
    private GameObject currentController = null;

    public bool IsInteractionAllowed()
    {
        return _allowInteraction && !hasControl;
    }

    public bool HasInteractionControl(GameObject controller)
    {
        return hasControl
               && currentController != null && controller == currentController;
    }

    public void AcquireControl(GameObject controllingObject)
    {
        if (controllingObject == null)
        {
            return;
        }

        currentController = controllingObject;
        hasControl = true;
    }

    public void ReleaseControl()
    {
        currentController = null;
        hasControl = false;
    }
}