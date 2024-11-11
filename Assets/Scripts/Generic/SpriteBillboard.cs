using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool rotateY = false;

    private void Update()
    {
        // Ensure player is available
        if (LevelManager.Instance.playerSpawn.playerSpawned == null) return;

        // Get the player position
        Transform playerTransform = LevelManager.Instance.playerSpawn.playerSpawned.transform;

        // Calculate the direction to the player
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        // If rotating only on the XZ plane, remove the y component
        if (!rotateY) directionToPlayer.y = 0;

        // Calculate the target rotation to face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // Apply the rotation
        transform.rotation = targetRotation;
    }
}