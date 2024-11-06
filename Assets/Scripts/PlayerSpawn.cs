using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [HideInInspector] public PlayerController playerSpawned;

    void Awake()
    {
        if (player == null)
        {
            Debug.LogError("Player prefab is not assigned in the PlayerSpawn script.");
            return;
        }
    }

    public void SpawnPlayer(List<WeaponSpawn> weaponsToSpawn)
    {
        DestroyExistingPlayers();

        CapsuleCollider playerCollider = player.GetComponent<CapsuleCollider>();
        if (playerCollider == null)
        {
            Debug.LogError("Player prefab does not have a CapsuleCollider component.");
            return;
        }

        Vector3 spawnOffset = new Vector3(0, playerCollider.height / 2, 0);
        playerSpawned = Instantiate(player, transform.position + spawnOffset, transform.rotation).GetComponent<PlayerController>();

        foreach(WeaponSpawn weaponToSpawn in weaponsToSpawn)
        {
            Weapon weapon = Instantiate(weaponToSpawn.weaponPrefab);

            weapon.playerController = playerSpawned;
            weapon.canPickup = false;
            weapon.startingAmmo = weaponToSpawn.startingAmmo;

            //this will attach it to the weapon holder game object and add it to the weapons array
            if (playerSpawned.weaponHolder.AddWeapon(weapon, true))
            {
                //if is new weapon lets equip it
                weapon.Attach();
            }
        }
    }

    private void DestroyExistingPlayers()
    {
        // Find all existing players in the scene
        PlayerController[] existingPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        // Loop through and destroy each player
        foreach (PlayerController existingPlayer in existingPlayers)
        {
            Destroy(existingPlayer.gameObject);
        }
    }
}
