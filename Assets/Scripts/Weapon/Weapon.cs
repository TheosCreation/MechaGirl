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

    [Header("Projectile Settings")]
    [SerializeField] private bool useProjectiles = true;
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

        if (true)
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
            projectile.Initialize(damage, target, playerController);
        }
    }

    private void FireHitscan()
    {
        // Fire a raycast from the firing point
        
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