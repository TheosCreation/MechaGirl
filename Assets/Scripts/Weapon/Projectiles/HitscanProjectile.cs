using Runtime;
using System;
using UnityEngine;

public class HitscanProjectile : Projectile
{
    [Tab("Setup")]
    [SerializeField] protected float headShotMultiplier = 1.5f;
    [SerializeField] protected float hitParticlesLifetime = 1.0f;
    [SerializeField] protected float particleOffset = 0.1f;
    [SerializeField] protected GameObject gunTrailPrefab;

    [Tab("Audio")]
    [SerializeField] protected float audioVolume = 0.1f;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
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
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            damageable = hit.collider.GetComponent<IDamageable>();
        }

        if (damageable != null)
        {
            m_weaponUser.OnHit();

            //Spawn hit enemy particle and play enemy hit sound also damage enemy
            Vector3 spawnPosition = hit.point + hit.normal * particleOffset;
            if (hit.collider.gameObject.CompareTag("Head"))
            {
                damageable.Damage(damage * headShotMultiplier);
                HitDamageable(spawnPosition, hit.normal, GameManager.Instance.prefabs.hitEnemyPrefab, GameManager.Instance.prefabs.enemyWeakspotHitSound);
            }
            else
            {
                damageable.Damage(damage);
                HitDamageable(spawnPosition, hit.normal, GameManager.Instance.prefabs.hitEnemyPrefab, GameManager.Instance.prefabs.enemyHitSound);
            }

        }
        else
        {
            //Spawns particles and sound
            Vector3 spawnPosition = hit.point + hit.normal * 0.1f;
            HitWall(spawnPosition, hit.normal);
        }
        Destroy(gameObject);
    }
}
