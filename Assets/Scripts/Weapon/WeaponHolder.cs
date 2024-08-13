using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private Weapon[] weapons;
    public Weapon currentWeapon;
    private int currentWeaponIndex = 0;

    private void Awake()
    {
        InputManager.Instance.playerInput.InGame.WeaponSwitch.performed += ctx => WeaponSwitch(ctx.ReadValue<Vector2>());
        InputManager.Instance.playerInput.InGame.Shoot.started += _ctx => currentWeapon.StartShooting();
        InputManager.Instance.playerInput.InGame.Shoot.canceled += _ctx => currentWeapon.EndShooting();
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
}