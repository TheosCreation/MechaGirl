using UnityEngine;
using UnityEngine.Events;

public class TriggerCheckPoint : IResetable
{
    private bool active = true;
    private SpriteRenderer spriteRenderer;
    private BoxCollider boxCollider;
    public WeaponSpawner[] weaponSpawners;
    public UnityEvent OnCheckPoint;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if(active)
        {
            active = false;
            LevelManager.Instance.SetCheckPoint(this);
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;
            OnCheckPoint?.Invoke();
            foreach (WeaponSpawner weaponSpawner in weaponSpawners)
            {
                weaponSpawner.DeActivate();
            }
        }
    }

    public override void Reset()
    {
        active = true;
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
    }
}