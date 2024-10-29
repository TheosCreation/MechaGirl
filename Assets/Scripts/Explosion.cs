using UnityEngine;
using UnityEngine.Audio;

public class Explosion : MonoBehaviour
{
    public float despawnTimer = 2.0f;
    public float deactivateTime = 0.1f;
    public float damage = 1.0f;
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
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null )
        {
            damageable.Damage( damage );
        }
    }
}