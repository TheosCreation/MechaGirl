using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float despawnTimer = 2.0f;
    public float deactivateTime = 0.1f;
    public float damage = 1.0f;
    public bool fromEnemy = true;
    private AudioSource audioSource;
    private Collider triggerCollider;
    private Timer deactivateTimer;
    [SerializeField] private AudioClip explosionSound;

    [Header("Screen Shake")]
    [SerializeField] private float screenShakeDuration = 0.1f;
    [SerializeField] private float screenShakeAmount = 0.1f;

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
            // If the enemy shoots a explosion it cannot damage other enemies or itself
            if (!fromEnemy || (fromEnemy && other.CompareTag("Player")))
            {
                damageable.Damage(damage); var Player = other.GetComponent<PlayerLook>();
                if (Player != null)
                {
                    Player.TriggerScreenShake(screenShakeDuration, screenShakeAmount);
                }
            }
        }
    }
}