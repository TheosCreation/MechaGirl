using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private PlayerController playerController;
    private List<Weapon> weapons = new List<Weapon>();
    public Weapon currentWeapon;
    private Weapon lastThrowWeapon;
    private int currentWeaponIndex = 0;
    [SerializeField] private float throwForce = 10.0f;
    [SerializeField] private float pickUpDelay = 1.0f;
    private bool isShooting = false;
    private bool isSwitching = false;
    private float scrollSwitchDelay = 0.2f;
    public ParticleSystem pickupParticle;
    [SerializeField] protected AudioClip pickUpSound;
    private AudioSource pickupAudioSource;

    private void Awake()
    {
        InputManager.Instance.playerInput.InGame.WeaponSwitch.performed += ctx => WeaponSwitch(ctx.ReadValue<Vector2>());
        InputManager.Instance.playerInput.InGame.Shoot.started += _ctx => StartShooting();
        InputManager.Instance.playerInput.InGame.Shoot.canceled += _ctx => EndShooting();
        InputManager.Instance.playerInput.InGame.WeaponThrow.started += _ctx => TryThrowWeapon();
        InputManager.Instance.playerInput.InGame.WeaponPickUp.started += _ctx => DashAbility();
        playerController = GetComponentInParent<PlayerController>();
        pickupAudioSource = GetComponent<AudioSource>();

        // Initialize weapons from children
        weapons.AddRange(GetComponentsInChildren<Weapon>());
    }

    void Start()
    {
        // Initialize the first weapon if needed
        if (weapons.Count > 0)
        {
            SelectWeapon(currentWeaponIndex);
        }
    }

    private void EndShooting()
    {
        isShooting = false;
    }

    private void StartShooting()
    {
        isShooting = true;
    }

    public void FixedUpdate()
    {
        if (currentWeapon != null)
        {
            currentWeapon.isShooting = isShooting;
        }
    }

    private void WeaponSwitch(Vector2 direction)
    {
        if (!isSwitching)
        {
            StartCoroutine(WeaponSwitchDelay(direction));
        }
    }

    private IEnumerator WeaponSwitchDelay(Vector2 direction)
    {
        isSwitching = true;
        if (direction.y > 0)
        {
            // Scroll up, switch to the next weapon
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        }
        else if (direction.y < 0)
        {
            // Scroll down, switch to the previous weapon
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
        }

        SelectWeapon(currentWeaponIndex);
        yield return new WaitForSeconds(scrollSwitchDelay);
        isSwitching = false;
    }
    private void DashToWeapon()
    {
        if (lastThrowWeapon != null)
        {
            playerController.playerMovement.Teleport(lastThrowWeapon.transform.position);
            lastThrowWeapon.PickUp(this, playerController, true);
        }
    }
    private void DashAbility()
    {
        Debug.Log("daash");
        currentWeapon.UseAbility();
        DashToWeapon();
    }

    public void TryThrowWeapon()
    {
        if (currentWeaponIndex != 0)
        {
            Weapon weaponToThrow = currentWeapon;

            SelectWeapon(0);

            weaponToThrow.Throw(playerController.playerCamera.transform.forward, throwForce, pickUpDelay);
            Remove(weaponToThrow);

            lastThrowWeapon = weaponToThrow;
        }
        else
        {
            DashToWeapon();
        }
    }

    // Weapon pickup method
    public bool AddWeapon(Weapon weapon, bool ignoreParticles)
    {
        if (lastThrowWeapon == weapon)
        {
            lastThrowWeapon = null;
        }

        // Check if the weapon already exists
        foreach (Weapon existingWeapon in weapons)
        {
            if (existingWeapon.GetType() == weapon.GetType())
            {
                // Add ammo to the existing weapon
                existingWeapon.Ammo += weapon.startingAmmo;

                // Destroy the new weapon to avoid duplicates
                Destroy(weapon.gameObject);

                return false; // Don't auto-equip if just adding ammo
            }
        }

        // Add the new weapon to the list
        weapons.Add(weapon);

        if (!ignoreParticles)
        {
            pickupParticle.Play();
            pickupAudioSource.PlayOneShot(pickUpSound);
        }

        weapon.enabled = false; // Disable the weapon script as the game object stays active
        weapon.transform.parent = transform; // Attach the weapon to the weapon holder again
        weapon.transform.localPosition = Vector3.zero; // Reset position

        // Auto-equip the newly added weapon
        SelectWeapon(weapons.Count - 1);

        return true;
    }

    private void SelectWeapon(int index)
    {
        // Logic to activate the selected weapon and deactivate others
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].enabled = i == index;
            weapons[i].isShooting = false;

            if (i == index)
            {
                currentWeaponIndex = index;
                currentWeapon = weapons[i];

                UiManager.Instance.UpdateWeaponImage(currentWeapon.Sprite);
                UiManager.Instance.UpdateWeaponIcon(currentWeapon.iconSprite);
            }
        }
    }

    public void Remove(Weapon weapon)
    {
        if (weapons.Contains(weapon))
        {
            weapons.Remove(weapon);
        }
        else
        {
            Debug.LogWarning("Weapon not found in the list.");
        }
    }

    public void SwitchToWeaponWithAmmo()
    {
        for (int i = 1; i < weapons.Count; i++)
        {
            // Check if the weapon has ammo
            if (weapons[i].Ammo > 0)
            {
                // Switch to this weapon
                SelectWeapon(i);
                return;
            }
        }
        SelectWeapon(0);
    }
}