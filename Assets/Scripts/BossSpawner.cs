using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private BossEnemy bossPrefab;
    [SerializeField] private Transform bossSpawnLocation;
    public List<Transform> enemySpawnLocations; // List of spawn locations
    public List<Enemy> enemyPrefabs;       // List of enemy prefabs
    public UnityEvent OnBossDead;
    private BossEnemy bossSpawned;

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
            // Select a random enemy prefab
            Enemy randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Spawn the enemy at the spawn location
            Enemy spawnedEnemy = Instantiate(randomEnemy, spawnLocation.position, spawnLocation.rotation);
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

    private void UpdateBossHealthBar()
    {
        if(bossSpawned != null)
        {
            UiManager.Instance.SetBossBarHealth(bossSpawned.Health / bossSpawned.maxHealth);
        }
    }

    public void Reset()
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
        OnBossDead.Invoke();
        bossSpawned.OnHealthChanged -= UpdateBossHealthBar;
        UiManager.Instance.SetBossBarStatus(false);

        // Optionally, you could nullify the bossSpawned reference after death
        bossSpawned = null;
    }
}
