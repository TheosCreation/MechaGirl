using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private bool reuseable = false;

    // UnityEvent that can be configured in the Inspector
    public UnityEvent onTriggerEnter;

    private bool active = true;

    // This method is called when another collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if(active || reuseable)
        {
            onTriggerEnter?.Invoke();
            active = false;
        }
    }
}