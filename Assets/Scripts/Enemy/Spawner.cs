using System;
using System.Collections;
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
            Debug.LogWarning("All waves have been completed. Wave count " + currentWaveIndex);
            return;
        }

        // Start coroutine to spawn enemies for the current wave
        StartCoroutine(SpawnEnemiesForWave(waves[currentWaveIndex]));
    }

    private IEnumerator SpawnEnemiesForWave(Wave wave)
    {
        int spawnedEnemies = 0;

        foreach (EnemySpawn enemySpawn in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                // Spawn each enemy with a delay to avoid performance spikes
                if (SpawnEnemy(enemySpawn.enemyPrefab))
                {
                    spawnedEnemies++;
                }

                yield return new WaitForSeconds(0.02f);
            }
        }

        if (spawnedEnemies == 0)
        {
            Debug.LogWarning("No enemies were spawned in this wave.");
        }
    }

    private bool SpawnEnemy(Enemy enemyPrefab)
    {
        const int maxRetries = 10; // Maximum attempts to find a valid spawn point
        int attemptCount = 0;

        while (attemptCount < maxRetries)
        {
            Vector3 randomPoint = GetRandomPointInArea();

            if (enemyPrefab.ignoreNavMeshOnSpawn)
            {
                // Instantiate the enemy at the random point
                Enemy enemy = Instantiate(enemyPrefab, randomPoint, Quaternion.identity);

                if (enemy != null)
                {
                    // Subscribe to the OnDeath event
                    enemy.OnDeath += HandleEnemyDeath;
                    activeEnemies.Add(enemy);

                    // Set the player as the target
                    PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
                    if (player != null)
                    {
                        enemy.SetTarget(player.transform);
                    }
                    return true;
                }
            }
            else
            {
                // Raycast down to find ground level
                float raycastHeight = 20.0f;
                Vector3 raycastOrigin = randomPoint;

                if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2, navMeshLayer))
                {
                    // Check if the hit point is on the NavMesh
                    if (IsPointOnNavMesh(hit.point))
                    {
                        // Instantiate the enemy at the hit point
                        Enemy enemy = Instantiate(enemyPrefab, hit.point, Quaternion.identity);

                        if (enemy != null)
                        {
                            // Subscribe to the OnDeath event
                            enemy.OnDeath += HandleEnemyDeath;
                            activeEnemies.Add(enemy);

                            // Set the player as the target
                            PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
                            if (player != null)
                            {
                                enemy.SetTarget(player.transform);
                            }
                            return true;
                        }
                    }
                }

                // Debugging information to track failed attempts
                Debug.LogWarning($"Attempt {attemptCount + 1}: Failed to find a valid point on the NavMesh at {randomPoint}. Trying again.");
            }

            attemptCount++;
        }

        Debug.LogError("Exceeded maximum retries. Failed to spawn enemy.");
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
            return;
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
            }
        }
    }

    public void CompleteSpawner()
    {
        spawnerComplete = true;
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
        if (!spawnerComplete)
        {
            StopAllCoroutines();

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
