using Runtime;
using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Tab("Settings")]
    [Header("Looks")]
    public Sprite Sprite;
    public Vector3 AnimationPosition = Vector3.zero;
    protected Vector3 currentAnimationPosition = Vector3.zero;
    public Vector3 AnimationRotation = Vector3.zero;
    protected Vector3 currentAnimationRotation = Vector3.zero;
    public Sprite iconSprite;
    public Sprite inGameSprite;
    [SerializeField] private AnimatorOverrideController gunPlayerController;
    [SerializeField] private AnimatorOverrideController gunInGameController;
    protected Sprite currentSprite;

    [Header("Equip")]
    [SerializeField] private bool isEquip = false;
    [SerializeField] private float equipTime = 0.5f;
    protected Timer equipTimer;

    [Header("Throwing")]
    [SerializeField] protected bool canThrow = true;

    [Header("Shooting")]
    [SerializeField] protected bool canShoot = true;
    public bool isShooting = false;
    [SerializeField] protected float shootsPerSecond = 1.0f;
    protected float shootTimer = 0.0f;
    private float quickShootTimer = 0.0f;

    [Header("Screen Shake")]
    [Range(0.0f, 0.1f)][SerializeField] protected float screenShakeDuration = 0.1f;
    [Range(0.0f, 0.1f)][SerializeField] protected float screenShakeAmount = 0.1f;

    [Header("Pick Up")]
    [SerializeField] protected bool canPickup = true; // moved up to settings tab so easily visable

    [Tab("Setup")]
    [Header("Projectile Settings")]
    public int startingAmmo = 10;

    [SerializeField] protected Projectile enemyProjectilePrefab;
    [SerializeField] protected Projectile playerProjectilePrefab;

    [Header("Audio")]
    [SerializeField] protected AudioSource shootingSource;
    [SerializeField] protected AudioClip[] shootingSounds;

    [HideInInspector] public float predictionTime = 0.2f;

    protected Animator animator;

    protected Transform target; // Target to aim at

    protected PlayerController playerController;
    [HideInInspector] public WeaponHolder WeaponHolder;

    protected SpriteRenderer spriteRenderer;
    protected BoxCollider bc; // lets keep the theme going, things are keeped private for a reason
    protected Rigidbody rb;
    protected Timer pickupTimer;
    protected Vector3 shotDirection;
    protected SpriteBillboard spriteBillboard;
    private Color weaponColor = Color.white;
    private int ammo;

    [SerializeField] private bool ignoreAmmo = false;
    public int Ammo
    {
        get => ammo;
        set
        {
            ammo = value;
            if (playerController != null && isActiveAndEnabled)
            {
                UiManager.Instance.UpdateAmmoUi(ammo);
            }
            if (!ignoreAmmo)
            {
                if (ammo <= 0 && isActiveAndEnabled && WeaponHolder != null)
                {
                    weaponColor = Color.red;

                }
                else if (ammo > 0 && weaponColor == Color.red)
                {
                    weaponColor = Color.white;
                }
                UiManager.Instance.UpdateWeaponImageColor(weaponColor);
            }
        }
    }

    public event Action OnAttack;

    protected void Awake()
    {
        animator = GetComponent<Animator>();

        equipTimer = gameObject.AddComponent<Timer>();
        pickupTimer = gameObject.AddComponent<Timer>();


        spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        spriteBillboard = GetComponent<SpriteBillboard>();
        playerController = GetComponentInParent<PlayerController>();

        ammo = startingAmmo;
    }

    protected void Start()
    {
        Ammo = startingAmmo;
    }

    protected void Update()
    {
        shootTimer -= Time.deltaTime;
        /*quickShootTimer -= Time.deltaTime;
        if (quickShootTimer < 0.0f && isShooting)
        {
            //this is just a slight delay to stop the enemy from rotating
           
        }*/

        if (shootTimer < 0.0f && isShooting)
        {

            canShoot = true;

        }

        if (canShoot && isShooting && isEquip)
        {
            if (Ammo > 0 || ignoreAmmo)
            {
                canShoot = false;


                shootTimer = CalculateFireRate();
                quickShootTimer = shootTimer - predictionTime;
                Shoot();
            }
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

        if (AnimationPosition != currentAnimationPosition)
        {
            currentAnimationPosition = AnimationPosition;
            if (playerController != null)
            {
                UiManager.Instance.UpdateWeaponAnimationPosition(currentAnimationPosition);
            }
        }

        if (AnimationRotation != currentAnimationRotation)
        {
            currentAnimationRotation = AnimationRotation;
            if (playerController != null)
            {
                UiManager.Instance.UpdateWeaponAnimationRotation(currentAnimationRotation);
            }
        }
    }

    private void OnEnable()
    {
        // We equip if the weapon has been selected and is held
        if (playerController == null) return;
        Equip();
    }

    private void OnDisable()
    {
        equipTimer.StopTimer();
        isEquip = false;
    }

    protected virtual void Attach()
    {
        if (Ammo > 0)
        {
            weaponColor = Color.white;
        }

        spriteBillboard.enabled = false;
        bc.enabled = false;
        rb.isKinematic = true;
        rb.useGravity = true;
        if (playerController != null)
        {
            spriteRenderer.enabled = false;
            animator.runtimeAnimatorController = gunPlayerController;
        }
        else
        {
            spriteRenderer.enabled = true;
            animator.runtimeAnimatorController = gunInGameController;
        }
    }

    protected void Equip()
    {
        Attach();
        animator.enabled = true;
        if (playerController != null)
        {
            if (!ignoreAmmo)
            {
                UiManager.Instance.UpdateAmmoUi(Ammo);
            }
            else
            {
                UiManager.Instance.UpdateAmmoUi(0);
            }

            spriteRenderer.enabled = false;
        }
        else
        {

            spriteRenderer.enabled = true;
        }

        animator.SetTrigger("Equip");

        equipTimer.SetTimer(equipTime, EquipFinish);
        UiManager.Instance.UpdateWeaponImageColor(weaponColor);
    }

    protected void EquipFinish()
    {
        isEquip = true;
    }

    public void Throw(Vector3 direction, float throwForce, float pickUpDelay)
    {
        transform.SetParent(null);

        transform.localScale = Vector3.one;
        //animator.runtimeAnimatorController = gunInGameController;

        rb.isKinematic = false;
        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);

        bc.enabled = true;
        animator.enabled = false;
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = inGameSprite;

        canPickup = false;
        //stop timer incase of repeat
        pickupTimer.StopTimer();
        pickupTimer.SetTimer(pickUpDelay, () => canPickup = true);

        isShooting = false;

        playerController = null;
        WeaponHolder = null;

        //clear attached functions
        OnAttack = null;

        spriteBillboard.enabled = true;
        //disable script just like unequiping the weapon
        this.enabled = false;
    }

    public void PickUp(WeaponHolder weaponHolder, PlayerController pc, bool ignorePickup)
    {
        if (!canPickup && !ignorePickup) return;

        playerController = pc;

        canPickup = false;

        //this will attach it to the weapon holder game object and add it to the weapons array
        if (weaponHolder.AddWeapon(this))
        {
            //if is new weapon lets equip it
            Attach();
        }
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
        shootTimer = CalculateFireRate();

        animator.SetTrigger("Shoot");

        if (playerController)
        {
            shotDirection = playerController.playerCamera.transform.forward;
        }
        else
        {
            OnAttack?.Invoke();

            shotDirection = transform.forward;
        }

        // Trigger screen shake if applicable
        if (playerController != null)
        {
            playerController.playerLook.TriggerScreenShake(screenShakeDuration, screenShakeAmount);
        }

        PlayRandomFiringSound();

        FireProjectile();

        if (playerController != null && !ignoreAmmo)
        {
            Ammo--;
        }
    }

    protected void FireProjectile()
    {
        if (enemyProjectilePrefab == null)
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

        Projectile projectile = null;
        if (playerController != null)
        {
            projectile = Instantiate(playerProjectilePrefab, transform.position, rotation);
        }
        else
        {
            projectile = Instantiate(enemyProjectilePrefab, transform.position, rotation);
        }
        if (projectile != null)
        {
            if (playerController)
            {
                projectile.owner = playerController.gameObject;

                projectile.Initialize(shotDirection, true);
                projectile.ownerLayer = playerController.gameObject.layer;

            }
            else
            {
                GameObject enemyRef = GetComponentInParent<Enemy>().gameObject;
                projectile.owner = enemyRef;
                projectile.ownerLayer = enemyRef.layer;
                projectile.Initialize(shotDirection, false);
            }
        }
    }

    protected void PlayRandomFiringSound()
    {
        if (shootingSounds.Length > 0 && shootingSource != null)
        {
            AudioClip randomClip = shootingSounds[UnityEngine.Random.Range(0, shootingSounds.Length)];
            shootingSource.PlayOneShot(randomClip);
        }
    }

    public void EndShooting()
    {
        isShooting = false;
    }

    public void UpdateWalkingAnimations(bool isWalking, float right)
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("Right", right);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
