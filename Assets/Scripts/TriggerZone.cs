using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : IResetable
{
    [SerializeField] private bool reuseable = false;

    // UnityEvent that can be configured in the Inspector
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private bool active = true;

    // This method is called when another collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if(active || reuseable)
        {
            onTriggerEnter?.Invoke();
            active = false;
        }
    }

    private void OnTriggerExit(Collider other)
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