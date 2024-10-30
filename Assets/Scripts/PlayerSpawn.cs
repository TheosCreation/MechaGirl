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

        // Spawn the new player
        SpawnPlayer();
    }

    public void SpawnPlayer()
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
