using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : IResetable
{
    [SerializeField] protected bool reuseable = false;

    // UnityEvent that can be configured in the Inspector
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private bool active = true;

    // This method is called when another collider enters the trigger zone
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if(active || reuseable)
        {
            onTriggerEnter?.Invoke();
            active = false;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        if (active || reuseable)
        {
            onTriggerExit?.Invoke();
            active = false;
        }
    }

    public override void Reset()
    {
        active = true;
    }
}