using Runtime;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Tab("Settings")]
    [Header("Looks")]
    public Sprite Sprite;
    public Sprite icon;
    protected Sprite currentSprite;

    [Header("Equip")]
    [SerializeField] private bool isEquip = false;
    [SerializeField] private float equipTime = 0.5f;
    protected Timer equipTimer;

    [Header("Throwing")]
    [SerializeField] protected bool canThrow = true;

    [Header("Shooting")]
    [SerializeField] protected bool canShoot = true;
    [SerializeField] protected bool isShooting = false;
    [SerializeField] protected float shootsPerSecond = 1.0f;
    protected float shootTimer = 0.0f;

    [Header("Screen Shake")]
    [Range(0.0f, 0.1f)][SerializeField] protected float screenShakeDuration = 0.1f;
    [Range(0.0f, 0.1f)][SerializeField] protected float screenShakeAmount = 0.1f;

    [Header("Pick Up")]
    [SerializeField] protected bool canPickup = true; // moved up to settings tab so easily visable

    [Tab("Setup")]
    [Header("Projectile Settings")]
    [SerializeField] protected int startingAmmo = 10;
    private int ammo;
    public int Ammo
    {
        get => ammo;
        set
        {
            ammo = value;
            if (playerController != null)
            {
                UiManager.Instance.UpdateAmmoUi(ammo);
            }

            if (ammo <= 0 && canThrow)
            {
                WeaponHolder.TryThrowWeapon();
            }
        }
    }

    [SerializeField] protected GameObject projectilePrefab;

    [Header("Audio")]
    [SerializeField] protected AudioSource shootingSource;
    [SerializeField] protected AudioClip[] shootingSounds;

    protected Animator animator;

    protected Transform target; // Target to aim at

    protected PlayerController playerController;
    protected SpriteRenderer spriteRenderer;
    public WeaponHolder WeaponHolder;
    protected BoxCollider bc; // lets keep the theme going, things are keeped private for a reason
    protected Rigidbody rb;
    protected Timer pickupTimer;

    protected void Awake()
    {
        animator = GetComponent<Animator>();

        equipTimer = gameObject.AddComponent<Timer>();
        pickupTimer = gameObject.AddComponent<Timer>();

        playerController = GetComponentInParent<PlayerController>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    protected void Start()
    {
        Ammo = startingAmmo;
    }

    protected void Update()
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
        Equip();
    }

    private void OnDisable()
    {
        equipTimer.StopTimer();
        isEquip = false;
    }

    protected void Equip()
    {
        bc.enabled = false;
        rb.isKinematic = true;
        animator.enabled = true;

        if (playerController != null)
        {
            spriteRenderer.enabled = false;
            UiManager.Instance.UpdateAmmoUi(Ammo);
        }

        animator.SetTrigger("Equip");

        equipTimer.SetTimer(equipTime, EquipFinish);
    }

    protected void EquipFinish()
    {
        isEquip = true;
    }

    public void Throw(Vector3 direction, float throwForce, float pickUpDelay)
    {
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.AddForce(direction * throwForce, ForceMode.Impulse);

        bc.enabled = true;
        spriteRenderer.enabled = true;
        animator.enabled = false;

        //stop timer incase of repeat
        pickupTimer.StopTimer();
        pickupTimer.SetTimer(pickUpDelay, () => canPickup = true);

        //disable script just like unequiping the weapon
        this.enabled = false;
    }

    public void PickUp(WeaponHolder weaponHolder)
    {
        if (!canPickup) return;
        if (Ammo <= 0)
        {
            Destroy(gameObject);
            return;
        }
        canPickup = false;

        //this will attach it to the weapon holder game object and add it to the weapons array
        weaponHolder.AddWeapon(this);

        //will disable unwanted stuff, will not play anims
        Equip();
    }

    protected float CalculateFireRate()
    {
        return 1 / shootsPerSecond;
    }

    public void StartShooting()
    {
        isShooting = true;
    }

    public virtual void Shoot()
    {
        if (playerController != null)
        {
            Ammo--;
        }

        animator.SetTrigger("Shoot");

        // Trigger screen shake if applicable
        if (playerController != null)
        {
            playerController.playerLook.TriggerScreenShake(screenShakeDuration, screenShakeAmount);
        }

        PlayRandomFiringSound();

        FireProjectile();
    }

    protected void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile has not been set");
            return;
        }
        Quaternion rotation = Quaternion.identity;
        if (playerController)
        {
            rotation = playerController.playerCamera.transform.rotation;
        }
        else
        {
            rotation = transform.rotation;
        }

        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            if (playerController)
            {
                projectile.Initialize(playerController.playerCamera.transform.forward, true);
            }
            else
            {
                projectile.Initialize(transform.transform.forward, false);
            }
        }
    }

    protected void PlayRandomFiringSound()
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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
