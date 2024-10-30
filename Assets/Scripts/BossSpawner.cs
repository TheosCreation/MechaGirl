using UnityEngine;
using UnityEngine.Events;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Enemy bossPrefab;
    [SerializeField] private Transform spawnLocation;

    public UnityEvent OnBossDead;
    private Enemy bossSpawned;

    public void SpawnBoss()
    {
        // Spawn the boss at the specified location
        bossSpawned = Instantiate(bossPrefab, spawnLocation.position, spawnLocation.rotation);
        bossSpawned.OnDeath += HandleBossDeath;

        // Set the target to a player by finding a GameObject with the "Player" tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
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
