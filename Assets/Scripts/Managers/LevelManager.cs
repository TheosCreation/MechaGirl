using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private float levelStartTime;
    private float levelCompleteTime;
    private bool isTimerRunning;
    [SerializeField] private Transform respawnTransform;
    private PlayerSpawn playerSpawn;
    [SerializeField] public GameObject tempCamera;
    [HideInInspector] public UnityEvent OnPlayerRespawn;
    [HideInInspector] public bool resetLevel = true;

    private List<TriggerDoor> triggerDoors = new List<TriggerDoor>();
    private List<TriggerZone> triggerZones = new List<TriggerZone>();
    private List<TriggerCheckPoint> checkPoints = new List<TriggerCheckPoint>();
    private List<Spawner> spawners = new List<Spawner>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Register all objects at the start
        triggerDoors.AddRange(FindObjectsByType<TriggerDoor>(FindObjectsSortMode.None));
        triggerZones.AddRange(FindObjectsByType<TriggerZone>(FindObjectsSortMode.None));
        checkPoints.AddRange(FindObjectsByType<TriggerCheckPoint>(FindObjectsSortMode.None));
        spawners.AddRange(FindObjectsByType<Spawner>(FindObjectsSortMode.None)); 
        
        playerSpawn = FindFirstObjectByType<PlayerSpawn>();
        if (playerSpawn == null)
        {
            Debug.LogAssertion("Player Spawn does not exist in scene cannot continue play");
        }
    }

    public void StartLevelTimer()
    {
        // Start the timer
        levelStartTime = Time.time;
        isTimerRunning = true;
    }

    public void CompleteLevel()
    {
        if (isTimerRunning)
        {
            // End the timer
            levelCompleteTime = Time.time - levelStartTime;
            isTimerRunning = false;

            PauseManager.Instance.SetPaused(true);
            PauseManager.Instance.canUnpause = false;


            // Get the best time for the current level
            float bestTimeForCurrentLevel = GameManager.Instance.GameState.GetBestTimeForCurrentLevel();

            if (levelCompleteTime < bestTimeForCurrentLevel)
            {
                // Update the best time for the current level
                GameManager.Instance.GameState.SetBestTimeForCurrentLevel(levelCompleteTime);

                // Save the game state
                GameManager.Instance.SerializeGameStateToJson();
            }

            // Pass the time to the UIManager and open the level complete screen
            UiManager.Instance.OpenLevelCompleteScreen(levelCompleteTime, GameManager.Instance.GameState.currentLevelIndex + 1);
        }
    }

    public float GetLevelCompleteTime()
    {
        return levelCompleteTime;
    }

    public void KillCurrentPlayer()
    {
        if (playerSpawn.playerSpawned)
        {
            playerSpawn.playerSpawned.Die();
        }
    }

    public void RespawnPlayer()
    {
        if (resetLevel)
        {
            ResetDoors(); 
            ResetTriggers();
            ResetCheckPoints();
            ResetSpawners();
        }
        else
        {
            OnPlayerRespawn?.Invoke();
        }

        UiManager.Instance.OpenPlayerHud();

        //reset player health reset scene
        if (respawnTransform != null)
        {
            playerSpawn.SpawnPlayer(respawnTransform.position, respawnTransform.rotation);
        }
        else
        {
            playerSpawn.SpawnPlayer(Vector3.zero, Quaternion.identity);
        }

        //reset doors, remove enemies, reset trigger zones
        //
        SettingsManager.Instance.player = playerSpawn.playerSpawned;

        if (tempCamera != null)
        {
            Destroy(tempCamera);
        }
        PauseManager.Instance.SetPaused(false);
        SettingsManager.Instance.ApplyAllSettings();

        //playerSpawn.playerSpawned.weaponHolder.SwitchToWeaponWithAmmo();
    }

    public void SetCheckPoint(Transform checkPointTransform)
    {
        respawnTransform.position = checkPointTransform.position;
        respawnTransform.rotation = checkPointTransform.rotation;
    }

    public void DestroyAllEnemies()
    {
        // Find all Enemy objects in the scene
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // Loop through and destroy each enemy GameObject
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }

    public void DestroyAllWeapons()
    {
        Weapon[] weapons = FindObjectsByType<Weapon>(FindObjectsSortMode.None);

        foreach (Weapon weapon in weapons)
        {
            Destroy(weapon.gameObject);
        }
    }

    public void ResetDoors()
    {
        foreach (TriggerDoor door in triggerDoors)
        {
            door.Unlock();
        }
    }

    public void ResetTriggers()
    {
        foreach (TriggerZone triggerZone in triggerZones)
        {
            triggerZone.Reset();
        }
    }
    
    public void ResetCheckPoints()
    {
        foreach (TriggerCheckPoint checkPoint in checkPoints)
        {
            checkPoint.Reset();
        }
    }

    public void ResetSpawners()
    {
        foreach(Spawner spawner in spawners)
        {
            spawner.Reset();
        }
    }
}