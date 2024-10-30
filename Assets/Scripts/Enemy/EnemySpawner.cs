using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Make sure to include this for NavMesh-related functionality

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform areaCenter; // Center of the spawning area
    [SerializeField] private Vector3 areaSize; // Size of the spawning area
    [SerializeField] private LayerMask navMeshLayer; // Layer mask for NavMesh checking
    [SerializeField] private float initialSpawnInterval = 5.0f; // Initial time between spawns
    [SerializeField] private float spawnIntervalDecreaseRate = 0.1f; // Rate at which the spawn interval decreases
    [SerializeField] private float minimumSpawnInterval = 1.0f; // Minimum time between spawns

    private float currentSpawnInterval;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentSpawnInterval);

            // Decrease the spawn interval but ensure it doesn't go below the minimum
            currentSpawnInterval = Mathf.Max(minimumSpawnInterval, currentSpawnInterval - spawnIntervalDecreaseRate);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 randomPoint = GetRandomPointInArea();

        // Check if the random point is on the NavMesh
        if (IsPointOnNavMesh(randomPoint))
        {

            GameObject enemy = Instantiate(GetRandomEnemyToSpawn(), randomPoint, Quaternion.identity);

            // Set the target to a player by finding a GameObject with the "Player" tag
            PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
            if (player != null)
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.SetTarget(player.transform);
                }
            }
        }
        else
        {
            // If the point is not on the NavMesh, try again
            SpawnEnemy();
        }
    }

    private GameObject GetRandomEnemyToSpawn()
    {
        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogWarning("No enemies set for spawning.");
            return null;
        }

        // Select a random index from the enemies array
        int randomIndex = Random.Range(0, enemies.Length);
        return enemies[randomIndex];
    }

    private Vector3 GetRandomPointInArea()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );

        return areaCenter.position + randomPoint;
    }

    private bool IsPointOnNavMesh(Vector3 point)
    {
        NavMeshHit hit;
        // Check if the point is on the NavMesh within a specified distance
        return NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas);
    }
}
