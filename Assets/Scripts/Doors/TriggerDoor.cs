using UnityEngine;
using UnityEngine.Events;

public class TriggerDoor : Door
{
    // Idk what these were gonna be used for
    //public UnityEvent onTriggerEnter;
    //private bool triggerEventActive = true;

    [SerializeField] private GameObject killOverlay;

    protected override void Start()
    {
        base.Start();

        UpdateKillLockOverlay();
    }

    public override void Lock()
    {
        base.Lock();

        UpdateKillLockOverlay();
    }

    public override void Unlock()
    {
        base.Unlock();

        UpdateKillLockOverlay();
    }

    private void UpdateKillLockOverlay()
    {
        if (killOverlay != null)
        {
            killOverlay.SetActive(locked);
        }
    }
}