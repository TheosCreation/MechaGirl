﻿using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [HideInInspector] public Camera playerCamera;
    [HideInInspector] public PlayerLook playerLook;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public WeaponHolder weaponHolder;

    public int maxHealth = 100;
    private float health;
    public bool isInvinsable = false;
    public float Health
    {
        get => health;
        set
        {
            health = value;

            UiManager.Instance.UpdateHealthUi(health);

            if (health <= 0)
            {
                if (isInvinsable) return;
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
        playerMovement = GetComponent<PlayerMovement>();
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

    public void Heal(float healAmount)
    {
        float newHealth = Health + healAmount;
        Health = Mathf.Clamp(newHealth, 0, maxHealth);
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
}
