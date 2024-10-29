using UnityEngine;
using UnityEngine.Audio;

public class Explosion : MonoBehaviour
{
    public float despawnTimer = 2.0f;
    public float damage = 1.0f;
    private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.PlayOneShot(explosionSound);
        Destroy(gameObject, despawnTimer);
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