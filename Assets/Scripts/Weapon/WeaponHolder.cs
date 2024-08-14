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

    private void Awake()
    {
        InputManager.Instance.playerInput.InGame.WeaponSwitch.performed += ctx => WeaponSwitch(ctx.ReadValue<Vector2>());
        InputManager.Instance.playerInput.InGame.Shoot.started += _ctx => currentWeapon.StartShooting();
        InputManager.Instance.playerInput.InGame.Shoot.canceled += _ctx => currentWeapon.EndShooting();
        InputManager.Instance.playerInput.InGame.WeaponThrow.started += _ctx => ThrowWeapon();
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
    void ThrowWeapon()
    {
        if(currentWeaponIndex!= 0) {
            lastThrowWeapon = currentWeapon;
            currentWeapon.transform.SetParent(null);
            Rigidbody rb = currentWeapon.gameObject.AddComponent<Rigidbody>();
            currentWeapon.gameObject.AddComponent<BoxCollider>();
            rb.isKinematic = false;
            currentWeapon.GetComponent<SpriteRenderer>().enabled = true;
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
            Remove(currentWeapon);
            SelectWeapon(0);
        }
        else if(lastThrowWeapon != null)
        {
            playerController.playerMovement.Teleport(lastThrowWeapon.transform.position + new Vector3(0.0f, 1.0f, 0.0f));
        }
    }
    private void SelectWeapon(int index)
    {
        // Logic to activate the selected weapon and deactivate others
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].enabled = i == index;
            if (i == index)
            {
                currentWeapon = weapons[i];
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