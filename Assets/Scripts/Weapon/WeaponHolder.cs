using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private PlayerController playerController;
    private Weapon[] weapons;
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
        InputManager.Instance.playerInput.InGame.WeaponPickUp.started += _ctx => DashToWeapon();
        playerController = GetComponentInParent<PlayerController>();
        pickupAudioSource = GetComponent<AudioSource>();
        weapons = GetComponentsInChildren<Weapon>();
    }
    void Start()
    {
        // Initialize the first weapon if needed
        SelectWeapon(currentWeaponIndex);
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
        currentWeapon.isShooting = isShooting;
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
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        }
        else if (direction.y < 0)
        {
            // Scroll down, switch to the previous weapon
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
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

    //weapon pickup is happening in player controller, because a box collider set to trigger is attach to root object of the player
    public bool AddWeapon(Weapon weapon, bool ignoreParticles)
    {
        if (lastThrowWeapon == weapon)
        {
            lastThrowWeapon = null;
        }
    
        foreach (Weapon existingWeapon in weapons)
        {
            if (existingWeapon.GetType() == weapon.GetType())
            {
                // Add ammo to the existing weapon
                existingWeapon.Ammo += weapon.startingAmmo;

                // Destroy the new weapon to avoid duplicates
                Destroy(weapon.gameObject);

                return false;
            }
        }

        // Create a new array with an additional slot
        Weapon[] newWeaponsArray = new Weapon[weapons.Length + 1];

        // Copy the existing weapons to the new array
        for (int i = 0; i < weapons.Length; i++)
        {
            newWeaponsArray[i] = weapons[i];
        }

        // Add the new weapon to the end
        newWeaponsArray[weapons.Length] = weapon;
        if (!ignoreParticles)
        {
            pickupParticle.Play();
            pickupAudioSource.PlayOneShot(pickUpSound);
        }

        // Replace the old array with the new one
        weapons = newWeaponsArray;

        weapon.enabled = false; //disable the weapon script as the game object stays active
        weapon.gameObject.transform.parent = transform; //attach the weapon to the weapon holder again
        weapon.gameObject.transform.localPosition = Vector3.zero; //then reset the position
        if (currentWeapon != null && weapons.Length > 0 && weapons[0] != null)
        {
            if (currentWeapon.GetType() == weapons[0].GetType())
            {
                SelectWeapon(weapons.Length - 1);
            }
        }

        return true;
    }

    private void SelectWeapon(int index)
    {
        // Logic to activate the selected weapon and deactivate others
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = i == index;
            weapons[i].isShooting = false;
            if (i == index)
            {
                currentWeaponIndex = index;
                currentWeapon = weapons[i];
                currentWeapon.WeaponHolder = this;
      
                UiManager.Instance.UpdateWeaponImage(currentWeapon.Sprite);
                UiManager.Instance.UpdateWeaponIcon(currentWeapon.iconSprite);
            }
        }
    }

    public void Remove(Weapon weapon)
    {
        int indexToRemove = -1;

        // Find the index of the weapon to remove
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapon == weapons[i])
            {
                indexToRemove = i;
                break;
            }
        }

        if (indexToRemove != -1)
        {
            // Create a new array with one less element
            Weapon[] newWeapons = new Weapon[weapons.Length - 1];
            for (int i = 0, j = 0; i < weapons.Length; i++)
            {
                if (i != indexToRemove)
                {
                    newWeapons[j++] = weapons[i];
                }
            }
            weapons = newWeapons;
        }
        else
        {
            Debug.LogWarning("Weapon not found in the array.");
        }
    }

    public void SwitchToWeaponWithAmmo()
    {
        for (int i = 1; i < weapons.Length; i++)
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