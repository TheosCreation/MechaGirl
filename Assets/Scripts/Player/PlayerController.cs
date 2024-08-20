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
    public float Health
    {
        get => health;
        set
        {
            health = value;

            UiManager.Instance.UpdateHealthUi(health);

            if (health <= 0)
            {
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

    private void Die()
    {
        PauseManager.Instance.PauseNoScreen();
        UiManager.Instance.OpenDeathScreen();

        LevelManager.Instance.DestroyAllEnemies();
        LevelManager.Instance.DestroyAllWeapons();

        RemoveCamera();

        Destroy(gameObject);
    }
    private void RemoveCamera()
    {
        LevelManager.Instance.tempCamera = GetComponentInChildren<Camera>().gameObject;
        LevelManager.Instance.tempCamera.transform.SetParent(null);
    }
}
