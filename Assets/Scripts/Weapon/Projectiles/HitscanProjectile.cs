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

    public override void Initialize(Vector3 startPosition, Vector3 direction, WeaponUser weaponUser)
    {
        direction = direction.normalized;
        base.Initialize(startPosition, direction, weaponUser);

        Ray ray = new Ray(startPosition, direction);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
        {
            targetPoint = hit.point;
            HandleHit(hit);
        }
        else
        {
            targetPoint = startPosition + direction * 1000f;
        }

        Debug.DrawRay(startPosition, ray.direction, Color.red, 1.0f);

        if (gunTrailPrefab != null)
        {
            GameObject gunTrailObject = Instantiate(gunTrailPrefab, transform.position, Quaternion.LookRotation(direction));
            TrailMovement trail = gunTrailObject.GetComponent<TrailMovement>();
            trail.hitpoint = targetPoint;
            trail.hitnormal = hit.normal;
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        GameObject hitParticles;
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            damageable = hit.collider.GetComponent<IDamageable>();
        }

        if (damageable != null)
        {
            m_weaponUser.OnHit();

            GameObject soundMakerObject = new GameObject("SoundMaker");
            soundMakerObject.transform.position = hit.point;
            SoundMaker soundMaker = soundMakerObject.AddComponent<SoundMaker>();
            if (hit.collider.gameObject.CompareTag("Head"))
            {
                damageable.Damage(damage * headShotMultiplier);
                soundMaker.PlaySound(enemyWeakspotHitSound, 0.1f);
            }
            else
            {
                damageable.Damage(damage);
                soundMaker.PlaySound(enemyHitSound, 0.1f);
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
}
