using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float despawnTimer = 2.0f;
    public float deactivateTime = 0.1f;
    public float damage = 1.0f;
    public bool enemy = true;
    private AudioSource audioSource;
    private Collider triggerCollider;
    private Timer deactivateTimer;
    [SerializeField] private AudioClip explosionSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        triggerCollider = GetComponent<Collider>();
        deactivateTimer = GetComponent<Timer>();
    }

    private void Start()
    {
        audioSource.PlayOneShot(explosionSound);
        Destroy(gameObject, despawnTimer);
        deactivateTimer.SetTimer(deactivateTime, Deactivate);
    }

    private void Deactivate()
    {
        triggerCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>()  ;
        if (damageable == null)
        {
            damageable = other.GetComponent<IDamageable>();
        }
        if (damageable != null )
        {
            if (other.CompareTag("Player") == enemy)
            {
                damageable.Damage(damage);
            }
        }
    }
}