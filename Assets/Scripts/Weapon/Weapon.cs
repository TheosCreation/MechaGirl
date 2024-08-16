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

    [Header("Throwing")]
    [SerializeField] private bool canThrow = true;

    [Header("Shooting")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private float shootsPerSecond = 1.0f;
    private float shootTimer = 0.0f;

    [Header("Screen Shake")]
    [Range(0.0f, 0.1f)][SerializeField] private float screenShakeDuration = 0.1f;
    [Range(0.0f, 0.1f)][SerializeField] private float screenShakeAmount = 0.1f;

    [Header("Pick Up")]
    [SerializeField] private bool canPickup = true; // moved up to settings tab so easily visable

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
            if(playerController != null)
            {
                UiManager.Instance.UpdateAmmoUi(ammo);
            }

            if (ammo <= 0 && canThrow)
            {
                WeaponHolder.TryThrowWeapon();
            }
        }
    }

    [SerializeField] private GameObject projectilePrefab;

    [Header("Audio")]
    [SerializeField] private AudioSource shootingSource;
    [SerializeField] private AudioClip[] shootingSounds;

    protected Animator animator;

    private Transform target; // Target to aim at

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    public WeaponHolder WeaponHolder;
    private BoxCollider bc; // lets keep the theme going, things are keeped private for a reason
    private Rigidbody rb;
    private Timer pickupTimer;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        equipTimer = gameObject.AddComponent<Timer>();
        pickupTimer = gameObject.AddComponent<Timer>();

        playerController = GetComponentInParent<PlayerController>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        Ammo = startingAmmo;
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
        Equip();
    }

    private void OnDisable()
    {
        equipTimer.StopTimer();
        isEquip = false;
    }

    private void Equip()
    {
        bc.enabled = false;
        rb.isKinematic = true;
        animator.enabled = true;

        if(playerController != null)
        {
            spriteRenderer.enabled = false;
        }

        animator.SetTrigger("Equip");

        equipTimer.SetTimer(equipTime, EquipFinish);

        if (playerController != null)
        {
            UiManager.Instance.UpdateAmmoUi(Ammo);
        }
    }

    private void EquipFinish()
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
        if(!canPickup) return;
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

    float CalculateFireRate()
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

    private void FireProjectile()
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
            if(playerController)
            {
                projectile.Initialize(playerController.playerCamera.transform.forward, true);
            }
            else
            {
                projectile.Initialize(transform.transform.forward, false);
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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

