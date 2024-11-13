using Runtime;
using UnityEngine;

public class HitscanProjectile : Projectile
{
    [Tab("Setup")]
    [SerializeField] protected float headShotMultiplier = 1.5f;
    [SerializeField] protected float hitParticlesLifetime = 1.0f;
    [SerializeField] protected float particleOffset = 0.1f;
    [SerializeField] protected GameObject hitWallPrefab;
    [SerializeField] protected GameObject hitEnemyPrefab;
    [SerializeField] protected GameObject gunTrailPrefab;

    [Tab("Audio")]
    [SerializeField] protected AudioClip enemyHitSound;
    [SerializeField] protected AudioClip enemyWeakspotHitSound;
    [SerializeField] protected AudioClip wallHitSound;
    [SerializeField] protected float audioVolume = 0.1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from this GameObject.");
        }
    }

    public override void Initialize(Vector3 startPosition, Vector3 direction, bool fromPlayer)
    {
        base.Initialize(startPosition, direction, fromPlayer);
        if (!fromPlayer)
        {
            RemoveEnemyFromHitMask();
        }
        Ray ray = new Ray(startPosition, direction);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
        {
            targetPoint = hit.point;
            HandleHit(hit, fromPlayer);
        }
        else
        {
            targetPoint = startPosition + direction * 1000f;
        }

        Debug.DrawRay(startPosition, ray.direction, Color.red, 1.0f);

        if (gunTrailPrefab != null)
        {
            GameObject gunTrailObject = Instantiate(gunTrailPrefab, transform.position, Quaternion.LookRotation(targetPoint - transform.position));
            TrailMovement trail = gunTrailObject.GetComponent<TrailMovement>();
            trail.hitpoint = targetPoint;
            trail.hitnormal = hit.normal;
        }
    }

    private void HandleHit(RaycastHit hit, bool fromPlayer)
    {
        GameObject hitParticles;
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            damageable = hit.collider.GetComponent<IDamageable>();
        }

        if (damageable != null)
        {
            if (fromPlayer)
            {
                UiManager.Instance.FlashHitMarker();
            }

            if (hit.collider.gameObject.CompareTag("Head"))
            {
                damageable.Damage(damage * headShotMultiplier);
                PlayHitSound(hit.point, enemyWeakspotHitSound, damage);
            }
            else
            {
                damageable.Damage(damage);
                PlayHitSound(hit.point, enemyHitSound, damage);
            }


            if (hitEnemyPrefab != null)
            {
                Vector3 spawnPosition = hit.point + hit.normal * particleOffset;
                hitParticles = Instantiate(hitEnemyPrefab, spawnPosition, Quaternion.LookRotation(-hit.normal));
                Destroy(hitParticles.gameObject, hitParticlesLifetime);
            }
        }
        else
        {
            if (hitWallPrefab != null)
            {
                Vector3 spawnPosition = hit.point + hit.normal * particleOffset;
                hitParticles = Instantiate(hitWallPrefab, spawnPosition, Quaternion.LookRotation(-hit.normal));
                Destroy(hitParticles.gameObject, hitParticlesLifetime);
            }
          //  PlayHitSound(hit.point, wallHitSound, 1f);
        }
        Destroy(gameObject);
    }

    private void PlayHitSound(Vector3 position, AudioClip sound, float volumeByDamage)
    {
        if (sound != null && audioSource != null)
        {
            GameObject soundObject = new GameObject("SoundPlayer");
            soundObject.transform.position = position;
            AudioSource newAudioSource = soundObject.AddComponent<AudioSource>();
       
            newAudioSource.clip = sound;
            newAudioSource.priority = audioSource.priority;
            newAudioSource.volume = volumeByDamage * audioVolume;
            newAudioSource.pitch = audioSource.pitch;
            newAudioSource.spatialBlend = audioSource.spatialBlend;
            newAudioSource.rolloffMode = audioSource.rolloffMode;
            newAudioSource.minDistance = audioSource.minDistance;
            newAudioSource.maxDistance = audioSource.maxDistance;
            newAudioSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            Debug.Log(newAudioSource.volume);


            newAudioSource.Play();
            Destroy(soundObject, sound.length);
        }
    }
}
