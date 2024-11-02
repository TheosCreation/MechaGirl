using System;
using System.Collections.Generic;
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

public class Spawner : IResetable
{
    public Vector3 localStartPosition;
    public Vector3 localEndPosition;

    [SerializeField] private Wave[] waves;
    [SerializeField] private LayerMask navMeshLayer;

    private int currentWaveIndex = 0;
    private int deadEnemyCount = 0;

    public UnityEvent OnAllEnemiesDead;
    private bool spawnerComplete = false;

    public TriggerDoor[] doors; 
    private List<Enemy> activeEnemies = new List<Enemy>();

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
            Debug.LogWarning("All waves have been completed. Wave count " + currentWaveIndex.ToString());
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
                activeEnemies.Add(enemy);

                PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
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

        // Check if waves array is initialized and has elements
        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("Waves array is not initialized or empty!");
            return;
        }

        // Check if the current wave index is valid
        if (currentWaveIndex >= waves.Length)
        {
            // Trigger the OnAllEnemiesDead event when all enemies are dead
            OnAllEnemiesDead?.Invoke();
            foreach (TriggerDoor door in doors)
            {
                door.Unlock();
            }
            return; // Exit if no more waves are available
        }

        // Check if all enemies in the current wave are dead
        int enemyCountInCurrentWave = GetEnemyCountInWave(waves[currentWaveIndex]);
        if (deadEnemyCount >= enemyCountInCurrentWave)
        {
            // Move to the next wave
            deadEnemyCount = 0;
            currentWaveIndex++;

            // Check if the next wave exists before starting it
            if (currentWaveIndex < waves.Length)
            {
                StartWave();
            }
            else
            {
                // Trigger the OnAllEnemiesDead event if all waves are completed
                OnAllEnemiesDead?.Invoke();
                spawnerComplete = true;
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

    public override void Reset()
    {
        if(!spawnerComplete)
        {
            foreach (Enemy enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.OnDeath -= HandleEnemyDeath;
                    Destroy(enemy.gameObject);
                }
            }
            activeEnemies.Clear();

            currentWaveIndex = 0;
            deadEnemyCount = 0;
        }
    }

}