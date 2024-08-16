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
 

    private void Awake()
    {
        InputManager.Instance.playerInput.InGame.WeaponSwitch.performed += ctx => WeaponSwitch(ctx.ReadValue<Vector2>());
        InputManager.Instance.playerInput.InGame.Shoot.started += _ctx => currentWeapon.StartShooting();
        InputManager.Instance.playerInput.InGame.Shoot.canceled += _ctx => currentWeapon.EndShooting();
        InputManager.Instance.playerInput.InGame.WeaponThrow.started += _ctx => TryThrowWeapon();
        playerController = GetComponentInParent<PlayerController>();

        weapons = GetComponentsInChildren<Weapon>();
    }

    void Start()
    {
        // Initialize the first weapon if needed
        SelectWeapon(currentWeaponIndex);
    }

    private void WeaponSwitch(Vector2 scrollInput)
    {
        if (scrollInput.y > 0)
        {
            // Scroll up, switch to the next weapon
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        }
        else if (scrollInput.y < 0)
        {
            // Scroll down, switch to the previous weapon
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
        }

        SelectWeapon(currentWeaponIndex);
    }

    public void TryThrowWeapon()
    {
        if (currentWeaponIndex != 0)
        {
            lastThrowWeapon = currentWeapon;
            currentWeapon.Throw(playerController.playerCamera.transform.forward, throwForce, pickUpDelay);

            Remove(currentWeapon);

            SelectWeapon(0);
        }
        else
        {
            if (lastThrowWeapon != null)
            {
                playerController.playerMovement.Teleport(lastThrowWeapon.transform.position + new Vector3(0.0f, 1.0f, 0.0f));
            }
        }
    }

    //weapon pickup is happening in player controller, because a box collider set to trigger is attach to root object of the player
    public void AddWeapon(Weapon weapon)
    {
        // Create a new array with an additional slot
        Weapon[] newWeaponsArray = new Weapon[weapons.Length + 1];

        // Copy the existing weapons to the new array
        for (int i = 0; i < weapons.Length; i++)
        {
            newWeaponsArray[i] = weapons[i];
        }

        // Add the new weapon to the end
        newWeaponsArray[weapons.Length] = weapon;

        // Replace the old array with the new one
        weapons = newWeaponsArray;

        weapon.enabled = false; //disable the weapon script as the game object stays active
        weapon.gameObject.transform.parent = transform; //attach the weapon to the weapon holder again
        weapon.gameObject.transform.localPosition = Vector3.zero; //then reset the position

        SelectWeapon(weapons.Length - 1);
    }

    private void SelectWeapon(int index)
    {
        // Logic to activate the selected weapon and deactivate others
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = i == index;
            if (i == index)
            {
                currentWeaponIndex = index;
                currentWeapon = weapons[i];
                currentWeapon.WeaponHolder = this;
                UiManager.Instance.UpdateWeaponImage(currentWeapon.Sprite);
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
}