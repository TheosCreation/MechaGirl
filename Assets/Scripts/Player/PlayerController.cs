﻿using System;
using UnityEngine;

public class PlayerController : WeaponUser, IDamageable
{
    [HideInInspector] public Camera playerCamera;
    [HideInInspector] public PlayerLook playerLook;
    [HideInInspector] public MovementController playerMovement;
    [HideInInspector] public WeaponHolder weaponHolder;
    [SerializeField] private AudioSource audio1;
    [SerializeField] private AudioSource audio2;

    public int maxHealth = 100;
    private float health;
    public bool isInvisible = false;
    public float Health
    {
        get => health;
        set
        {
            health = value;

            UiManager.Instance.UpdateHealthUi(health);

            if (health <= 0)
            {
                if (isInvisible) return;
                Die();
            }
        }
    }
    private Vector3 respawnPosition;

    public event Action OnDeath;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        playerLook = GetComponent<PlayerLook>();
        playerMovement = GetComponent<MovementController>();
        weaponHolder = GetComponentInChildren<WeaponHolder>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetMaxHealth();
    }

    public void SetMaxHealth()
    {
        Health = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
        UiManager.Instance.FlashHurtScreen();
    }

    public bool Heal(float healAmount)
    {
        float previousHealth = Health;
        float newHealth = Health + healAmount;
        Health = Mathf.Clamp(newHealth, 0, maxHealth);

        return Health > previousHealth;
    }

    public void Die()
    {
        PauseManager.Instance.PauseNoScreen();
        UiManager.Instance.OpenDeathScreen();

        LevelManager.Instance.DestroyAllEnemies();
        LevelManager.Instance.DestroyAllWeapons();

        RemoveCamera();
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
    private void RemoveCamera()
    {
        LevelManager.Instance.SetTempCamera(true);
    }

    public void SetAudio(bool active)
    {
        audio1.mute = !active;
        audio2.mute = !active;
    }

    public override Vector3 GetFirePoint()
    {
        return playerCamera.transform.position;
    }

    public override Vector3 GetForwardDirection()
    {
        return playerCamera.transform.forward;
    }

    public override LayerMask GetHitMask()
    {
        return LayerMask.GetMask("Enemy") | LayerMask.GetMask("Head") | LayerMask.GetMask("Default") | LayerMask.GetMask("Ground");
    }

    public override void OnWeaponFire(Weapon weapon)
    {
        playerLook.TriggerScreenShake(weapon.screenShakeDuration, weapon.screenShakeAmount);
    }

    public override void OnHit()
    {
        base.OnHit();

        UiManager.Instance.FlashHitMarker();
    }

    public override void OnAmmoChange(int currentAmmo)
    {
        UiManager.Instance.UpdateAmmoUi(currentAmmo);
    }

    public override void OnPickUp()
    {
        base.OnPickUp();
    }

    public override Projectile GetProjectilePrefab(Weapon weapon)
    {
        return weapon.playerProjectilePrefab;
    }

    public override void DashForward(float speed, float dashDuration)
    {
        playerMovement.Dash(playerCamera.transform.forward, speed, dashDuration, true);
    }

    //public override void OnSpriteChange(Sprite newSprite)
    //{
    //    UiManager.Instance.UpdateWeaponImage(newSprite);
    //}
}
