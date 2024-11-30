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
    public bool isShooting = false;
    [SerializeField] protected float shootsPerSecond = 1.0f;
    protected float shootTimer = 0.0f;

    [Header("Screen Shake")]
    [Range(0.0f, 0.1f)] public float screenShakeDuration = 0.1f;
    [Range(0.0f, 0.1f)] public float screenShakeAmount = 0.1f;

    [Header("Pick Up")]
    public bool canPickup = true; // moved up to settings tab so easily visable

    [Tab("Setup")]
    [Header("Projectile Settings")]
    public int startingAmmo = 10;
    [SerializeField] protected Casing casingToSpawn;
    [SerializeField] protected Transform casingSpawnTransform;

    [SerializeField] public Projectile enemyProjectilePrefab;
    [SerializeField] public Projectile playerProjectilePrefab;

    [Header("Audio")]
    [SerializeField] protected AudioSource shootingSource;
    [SerializeField] protected AudioClip[] shootingSounds;

    protected WeaponUser weaponUser;

    [HideInInspector] public float predictionTime = 0.2f;

    protected Animator animator;

    protected Transform target; // Target to aim at

    protected SpriteRenderer spriteRenderer;
    public BoxCollider bc; // lets keep the theme going, things are keeped private for a reason
    public Rigidbody rb;
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
            if (weaponUser is PlayerController && !ignoreAmmo)
            {
                ammo = value;

                //we only set the value if its not equip
                if(!isActiveAndEnabled) return;

                weaponUser.OnAmmoChange(ammo);

                if (ammo <= 0)
                {
                    weaponColor = Color.red;
                }
                else if (ammo > 0 && weaponColor == Color.red)
                {
                    weaponColor = Color.white;
                }
                //Need to fix this later we dont want a direct is player call and UiManager call here
                UiManager.Instance.UpdateWeaponImageColor(weaponColor);
            }
        }
    }

    public event Action OnAttack;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteBillboard = GetComponent<SpriteBillboard>();
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        equipTimer = gameObject.AddComponent<Timer>();
        pickupTimer = gameObject.AddComponent<Timer>();

        //Try and get weapon user if attached already
        Enemy enemy = GetComponentInParent<Enemy>();
        PlayerController pc = GetComponentInParent<PlayerController>();
        if(enemy != null)
        {
            weaponUser = enemy;
        }
        if (pc != null)
        {
            weaponUser = pc;
        }


        ammo = startingAmmo;
    }

    protected virtual void Start()
    {
        Ammo = startingAmmo;
    }

    protected void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer < 0.0f && isShooting && isEquip)
        {
            if (Ammo > 0 || ignoreAmmo)
            {
                shootTimer = CalculateFireRate();
                Shoot();
            }
        }

        if (currentSprite != Sprite)
        {
            currentSprite = Sprite;
            if(weaponUser is PlayerController)
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
            if (weaponUser is PlayerController)
            {
                UiManager.Instance.UpdateWeaponAnimationPosition(currentAnimationPosition);
            }
        }

        if (AnimationRotation != currentAnimationRotation)
        {
            currentAnimationRotation = AnimationRotation;
            if (weaponUser is PlayerController)
            {
                UiManager.Instance.UpdateWeaponAnimationRotation(currentAnimationRotation);
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

    public virtual void Attach()
    {
        if (Ammo > 0)
        {
            weaponColor = Color.white;
        }

        spriteBillboard.enabled = false;
        bc.enabled = false;
        rb.isKinematic = true;
        transform.localRotation = Quaternion.identity;
        if (weaponUser is PlayerController)
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
        animator.enabled = true;

        if (weaponUser is PlayerController)
        {
            if (!ignoreAmmo)
            {
                weaponUser.OnAmmoChange(Ammo);
            }
            else
            {
                weaponUser.OnAmmoChange(0);
            }

            UiManager.Instance.UpdateWeaponImageColor(weaponColor);

            spriteRenderer.enabled = false;
        }
        else
        {

           // spriteRenderer.enabled = true;
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

        //clear attached functions
        OnAttack = null;

        spriteBillboard.enabled = true;
        //disable script just like unequiping the weapon
        this.enabled = false;
    }

    public bool PickUp(WeaponHolder weaponHolder, WeaponUser user, bool ignorePickup)
    {
        if (!canPickup && !ignorePickup) return false;
        
        SetWeaponUser(user);

        canPickup = false;

        //this will attach it to the weapon holder game object and add it to the weapons array
        if (weaponHolder.AddWeapon(this, false))
        {
            //if is new weapon lets equip it
            Attach();
        }

        return true;
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

        weaponUser.OnWeaponFire(this);
        PlayRandomFiringSound();
        FireProjectile();
        SpawnCasing();

        //if enemy ammo doesnt decrease
        Ammo--;

        OnAttack?.Invoke();
    }

    public void SetWeaponUser(WeaponUser user)
    {
        weaponUser = user;
    }

    protected void FireProjectile()
    {
        Projectile projectile = Instantiate(weaponUser.GetProjectilePrefab(this), transform.position, Quaternion.identity);
        // projectile.hitMask = GetHitMask();
        projectile.owner = gameObject;
        projectile.ownerLayer = gameObject.layer;

        projectile.Initialize(weaponUser.GetFirePoint().position, weaponUser.GetForwardDirection(), weaponUser);
    }

    protected void SpawnCasing()
    {
        if (casingToSpawn != null)
        {
            Instantiate(casingToSpawn, casingSpawnTransform.position, casingSpawnTransform.rotation);
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
