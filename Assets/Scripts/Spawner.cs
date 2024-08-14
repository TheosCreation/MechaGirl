using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    public Vector3 localStartPosition;
    public Vector3 localEndPosition;

    [SerializeField] private int enemyCountToSpawn = 0;
    [SerializeField] private GameObject[] enemiesToSpawn;
    [SerializeField] private LayerMask navMeshLayer;

    private int deadEnemyCount = 0;

    public UnityEvent OnAllEnemiesDead;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Convert local positions to world positions for visualization
        Vector3 worldStartPosition = transform.TransformPoint(localStartPosition);
        Vector3 worldEndPosition = transform.TransformPoint(localEndPosition);

        // Calculate the size of the box in local space
        Vector3 size = worldEndPosition - worldStartPosition;

        // Draw the box in the scene view using world coordinates
        Gizmos.DrawWireCube(worldStartPosition + size / 2, size);
    }

    public void SpawnEnemies()
    {
        // Spawn the enemies so the total enemy count is met
        for (int i = 0; i < enemyCountToSpawn; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 randomPoint = GetRandomPointInArea();

        // Check if the random point is on the NavMesh
        if (IsPointOnNavMesh(randomPoint))
        {
            GameObject enemyObj = Instantiate(GetRandomEnemyToSpawn(), randomPoint, Quaternion.identity);
            Enemy enemy = enemyObj.GetComponent<Enemy>();

            if (enemy != null)
            {
                // Subscribe to the OnDeath event
                enemy.OnDeath += HandleEnemyDeath;

                // Set the target to a player by finding a GameObject with the "Player" tag
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    enemy.SetTarget(player.transform);
                }
            }
        }
        else
        {
            Debug.LogWarning("Failed to find a valid point on the NavMesh. No enemy spawned this iteration.");
        }
    }

    private GameObject GetRandomEnemyToSpawn()
    {
        if (enemiesToSpawn == null || enemiesToSpawn.Length == 0)
        {
            Debug.LogWarning("No enemies set for spawning.");
            return null;
        }

        // Select a random index from the enemies array
        int randomIndex = Random.Range(0, enemiesToSpawn.Length);
        return enemiesToSpawn[randomIndex];
    }

    private Vector3 GetRandomPointInArea()
    {
        // Calculate the min and max points in world space
        Vector3 worldStartPosition = transform.TransformPoint(localStartPosition);
        Vector3 worldEndPosition = transform.TransformPoint(localEndPosition);

        // Generate a random point within the defined bounds
        Vector3 randomPoint = new Vector3(
            Random.Range(worldStartPosition.x, worldEndPosition.x),
            Random.Range(worldStartPosition.y, worldEndPosition.y),
            Random.Range(worldStartPosition.z, worldEndPosition.z)
        );

        return randomPoint;
    }

    private bool IsPointOnNavMesh(Vector3 point)
    {
        NavMeshHit hit;
        // Check if the point is on the NavMesh within a specified distance
        return NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas);
    }

    private void HandleEnemyDeath()
    {
        deadEnemyCount++;

        if (deadEnemyCount >= enemyCountToSpawn)
        {
            // Trigger the OnAllEnemiesDead event when all enemies are dead
            OnAllEnemiesDead?.Invoke();
        }
    }
}