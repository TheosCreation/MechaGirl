using UnityEngine;
using UnityEngine.Events;

public class TriggerCheckPoint : MonoBehaviour
{
    public UnityEvent OnPlayerRespawn;
    private bool active = true;
    private SpriteRenderer spriteRenderer;
    private BoxCollider boxCollider;

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
            LevelManager.Instance.SetCheckPoint(transform);
            LevelManager.Instance.OnPlayerRespawn = OnPlayerRespawn;
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;
        }
    }

    public void Reset()
    {
        active = true;
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
    }
}