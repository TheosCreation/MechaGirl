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
    private List<Enemy> spawnedEnemies = new List<Enemy>(); // List to track spawned enemies

    private void Awake()
    {
        SpawnBoss();
    }

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

        // Subscribe to health changes to update the UI accordingly
        bossSpawned.OnHealthChanged += UpdateBossHealthBar;

        bossSpawned.SetActive(false);
    }

    public void ActivateBoss()
    {
        // Enable the boss health bar UI and initialize its health
        UiManager.Instance.SetBossBarStatus(true, bossPrefab.name);
        UiManager.Instance.SetBossBarHealth(1.0f);

        bossSpawned.SetActive(true);
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

            // Add to the list of spawned enemies
            spawnedEnemies.Add(spawnedEnemy);

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

        if (enemiesAlive <= 0 && bossSpawned == null)
        {
            //OnBossDead?.Invoke();
        }
    }

    private void UpdateBossHealthBar()
    {
        if (bossSpawned != null)
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

        // Destroy all remaining enemies
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        spawnedEnemies.Clear(); // Clear the list of spawned enemies
        enemiesAlive = 0; // Reset the enemy count

        SpawnBoss();

        // Hide the boss health bar
        UiManager.Instance.SetBossBarStatus(false);
    }


    private void HandleBossDeath()
    {
        UiManager.Instance.SetBossBarStatus(false);

        bossSpawned = null;

        // Kill all remaining enemies when the boss dies
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                enemy.Damage(1000f);
            }
        }

        spawnedEnemies.Clear(); // Clear the list since all enemies are now dead
        enemiesAlive = 0;

        OnBossDead?.Invoke();
    }
}
