using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BossSpawner : IResetable
{
    [SerializeField] private BossEnemy bossPrefab;
    [SerializeField] private Transform bossSpawnLocation;
    public List<Transform> enemySpawnLocations; // List of spawn locations
    public List<Enemy> enemyPrefabs;       // List of enemy prefabs
    public UnityEvent OnBossDead;
    private BossEnemy bossSpawned;
    private int enemiesAlive = 0;
    public int maxAllowedToBeAlive = 8;

    public void SpawnBoss()
    {
        // Spawn the boss at the specified location
        bossSpawned = Instantiate(bossPrefab, bossSpawnLocation.position, bossSpawnLocation.rotation);
        bossSpawned.OnDeath += HandleBossDeath;
        bossSpawned.spawner = this;

        PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
        if (player != null)
        {
            bossSpawned.SetTarget(player.transform);
        }

        // Enable the boss health bar UI and initialize its health
        UiManager.Instance.SetBossBarStatus(true, bossPrefab.name);

        // Subscribe to health changes to update the UI accordingly
        bossSpawned.OnHealthChanged += UpdateBossHealthBar;
        UiManager.Instance.SetBossBarHealth(1.0f);
    }

    public void SpawnEnemies()
    {
        foreach (Transform spawnLocation in enemySpawnLocations)
        {
            // Check if we can spawn another enemy based on the max allowed limit
            if (enemiesAlive >= maxAllowedToBeAlive)
            {
                break; // Stop spawning if we've reached the maximum limit
            }

            // Select a random enemy prefab
            Enemy randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Spawn the enemy at the spawn location
            Enemy spawnedEnemy = Instantiate(randomEnemy, spawnLocation.position, spawnLocation.rotation);
            spawnedEnemy.OnDeath += HandleEnemyDeath;
            enemiesAlive++;

            PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
            if (player != null)
            {
                spawnedEnemy.SetTarget(player.transform);
            }
            else
            {
                Debug.LogError("No player found");
            }
        }
    }

    private void HandleEnemyDeath()
    {
        enemiesAlive--;

        if(enemiesAlive <= 0 && bossSpawned == null)
        {
            OnBossDead?.Invoke();
        }
    }

    private void UpdateBossHealthBar()
    {
        if(bossSpawned != null)
        {
            UiManager.Instance.SetBossBarHealth(bossSpawned.Health / bossSpawned.maxHealth);
        }
    }

    public override void Reset()
    {
        if (bossSpawned != null)
        {
            // Unsubscribe to prevent memory leaks
            bossSpawned.OnDeath -= HandleBossDeath;

            // Destroy the current boss instance
            Destroy(bossSpawned.gameObject);
        }

        // Hide the boss health bar
        UiManager.Instance.SetBossBarStatus(false);
    }

    private void HandleBossDeath()
    {
        UiManager.Instance.SetBossBarStatus(false);

        bossSpawned = null;

        if (enemiesAlive <= 0)
        {
            OnBossDead?.Invoke();
        }
    }
}
