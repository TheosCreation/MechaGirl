using UnityEngine;

public class TriggerDoor : Door
{
    private bool triggerEventActive = true;

    protected override void Start()
    {
        base.Start();
    }
    

    protected override void Open(Collider other)
    {
        base.Open(other);

        if (triggerEventActive)
        {
            LevelManager.Instance.StartLevelTimer();
            triggerEventActive = false;
        }
    }
}