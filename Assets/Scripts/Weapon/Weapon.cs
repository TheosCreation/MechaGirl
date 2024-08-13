using Runtime;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Tab("Settings")]
    [Header("Looks")]
    public Sprite Sprite;
    private Sprite currentSprite;

    [Header("Equip")]
    [SerializeField] private bool isEquip = false;
    [SerializeField] private float equipTime = 0.5f;
    Timer equipTimer;

    [Header("Shooting")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private float shootsPerSecond = 1.0f;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private float headShotMultiplier = 1.5f;
    private float shootTimer = 0.0f;

    [Header("Screen Shake")]
    [Range(0.0f, 0.1f)][SerializeField] private float screenShakeDuration = 0.1f;
    [Range(0.0f, 0.1f)][SerializeField] private float screenShakeAmount = 0.1f;

    [Tab("Setup")]
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float hitParticlesLifetime = 1.0f;
    [SerializeField] private float particleOffset = 0.1f;
    [SerializeField] private GameObject hitWallPrefab;
    [SerializeField] private GameObject hitEnemyPrefab;
    [SerializeField] private GameObject gunTrailPrefab;

    [Header("Projectile Settings")]
    [SerializeField] private bool useProjectiles = false;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Audio")]
    [SerializeField] private AudioSource shootingSource;
    [SerializeField] private AudioClip[] shootingSounds;

    private Animator animator;
    private Transform firingPoint; // The point from which projectiles are fired
    private Transform target; // Target to aim at

    private PlayerController playerController;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        firingPoint = transform;

        equipTimer = gameObject.AddComponent<Timer>();

        playerController = GetComponentInParent<PlayerController>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer < 0.0f && isShooting)
        {
            canShoot = true;
        }

        if (canShoot && isShooting && isEquip)
        {
            canShoot = false;

            shootTimer = CalculateFireRate();
            Shoot();
        }

        if (currentSprite != Sprite)
        {
            currentSprite = Sprite;
            if (playerController != null)
            {
                UiManager.Instance.UpdateWeaponImage(currentSprite);
            }
            else
            {
                spriteRenderer.sprite = currentSprite;
            }
        }
    }

    private void OnEnable()
    {
        animator.SetTrigger("Equip");

        equipTimer.SetTimer(equipTime, Equip);
    }

    void Equip()
    {
        isEquip = true;
    }

    private void OnDisable()
    {
        equipTimer.StopTimer();
        isEquip = false;
    }

    float CalculateFireRate()
    {
        return 1 / shootsPerSecond;
    }

    public void StartShooting()
    {
        isShooting = true;
    }

    public void Shoot()
    {
        if (!isEquip) return;

        animator.SetTrigger("Shoot");

        // Trigger screen shake if applicable
        if (playerController != null)
        {
            playerController.playerLook.TriggerScreenShake(screenShakeDuration, screenShakeAmount);
        }

        PlayRandomFiringSound();

        if (useProjectiles)
        {
            FireProjectile();
        }
        else
        {
            FireHitscan();
        }
    }

    private void FireProjectile()
    {
        if (firingPoint == null || projectilePrefab == null) return;

        GameObject projectileObject = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(damage, target);
        }
    }

    private void FireHitscan()
    {
        // Fire a raycast from the firing point
        Ray ray = new Ray(firingPoint.position, firingPoint.forward);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
        {
            targetPoint = hit.point;
            HandleHit(hit);
        }
        else
        {
            targetPoint = firingPoint.position + firingPoint.forward * 1000f;
        }

        // Draw a debug ray to visualize the hitscan raycast
        Debug.DrawRay(firingPoint.position, firingPoint.forward * 1000f, Color.red, 1.0f);

        if (gunTrailPrefab != null)
        {
            GameObject gunTrailObject = Instantiate(gunTrailPrefab, firingPoint.position, Quaternion.LookRotation(targetPoint - firingPoint.position));
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
            if(playerController)
            {
                UiManager.Instance.FlashHitMarker();
            }
            if (hit.collider.gameObject.CompareTag("Body"))
            {
                damageable.Damage(damage);
            }
            else
            {
                damageable.Damage(damage * headShotMultiplier);
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
        }
    }

    private void PlayRandomFiringSound()
    {
        if (shootingSounds.Length > 0 && shootingSource != null)
        {
            AudioClip randomClip = shootingSounds[Random.Range(0, shootingSounds.Length)];
            shootingSource.PlayOneShot(randomClip);
        }
    }

    public void EndShooting()
    {
        isShooting = false;
    }

    public void UpdateWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void SetFiringPoint(Transform newFiringPoint)
    {
        firingPoint = newFiringPoint;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}