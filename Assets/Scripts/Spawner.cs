using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[Serializable]
struct EnemySpawn
{
    public Enemy enemyPrefab; // Make this public to be accessible
    public int count;         // Number of enemies to spawn
}

[Serializable]
struct Wave
{
    public EnemySpawn[] enemiesToSpawn; // List of enemy types and counts in the wave
}

public class Spawner : MonoBehaviour
{
    public Vector3 localStartPosition;
    public Vector3 localEndPosition;

    [SerializeField] private Wave[] waves;
    [SerializeField] private LayerMask navMeshLayer;

    private int currentWaveIndex = 0;
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

    public void StartWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.LogWarning("All waves have been completed.");
            return;
        }

        // Spawn the enemies for the current wave
        SpawnEnemiesForWave(waves[currentWaveIndex]);
    }

    private void SpawnEnemiesForWave(Wave wave)
    {
        int spawnedEnemies = 0;

        foreach (EnemySpawn enemySpawn in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                if (SpawnEnemy(enemySpawn.enemyPrefab))
                {
                    spawnedEnemies++;
                }
            }
        }

        if (spawnedEnemies == 0)
        {
            Debug.LogWarning("No enemies were spawned in this wave.");
        }
    }

    private bool SpawnEnemy(Enemy enemyPrefab)
    {
        Vector3 randomPoint = GetRandomPointInArea();

        // Check if the random point is on the NavMesh
        if (IsPointOnNavMesh(randomPoint))
        {
            // Instantiate the enemy at the random point
            Enemy enemy = Instantiate(enemyPrefab, randomPoint, Quaternion.identity);

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
                return true;
            }
        }

        Debug.Log("Failed to find a valid point on the NavMesh. Trying again.");
        return false;
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
        if (waves == null || waves.Length < currentWaveIndex)
        {
            Debug.LogError("Waves array is not initialized or empty!");
            return;
        }

        // Check if all enemies in the current wave are dead
        int enemyCountInCurrentWave = GetEnemyCountInWave(waves[currentWaveIndex]);
        if (deadEnemyCount >= enemyCountInCurrentWave)
        {
            // Move to the next wave
            deadEnemyCount = 0;
            currentWaveIndex++;

            if (currentWaveIndex >= waves.Length)
            {

                // Trigger the OnAllEnemiesDead event when all enemies are dead
                OnAllEnemiesDead?.Invoke();
            }
            else
            {
                StartWave();
            }
        }
    }

    private int GetEnemyCountInWave(Wave? wave)
    { 
        if (!wave.HasValue)
        {
            Debug.LogError("Wave is null!");
            return 0;
        }

        var actualWave = wave.Value;
        if (actualWave.enemiesToSpawn == null)
        {
            Debug.LogError("enemiesToSpawn list is null!");
            return 0;
        }

        int totalEnemies = 0;
        foreach (EnemySpawn enemySpawn in actualWave.enemiesToSpawn)
        {
            totalEnemies += enemySpawn.count;
        }
        return totalEnemies;
    }

}