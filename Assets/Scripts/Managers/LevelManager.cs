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

    private void Start()
    {
        playerSpawn = FindFirstObjectByType<PlayerSpawn>();
        if(playerSpawn == null)
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


            if(levelCompleteTime < GameManager.Instance.GameState.level1BestTime)
            {
                GameManager.Instance.GameState.level1BestTime = levelCompleteTime;

                //save the game state
                GameManager.Instance.SerializeJson();
            }

            // Pass the time to the UIManager and open the level complete screen
            UiManager.Instance.OpenLevelCompleteScreen(levelCompleteTime);
        }
    }

    public float GetLevelCompleteTime()
    {
        return levelCompleteTime;
    }

    public void RespawnPlayer()
    {
        if (OnPlayerRespawn == null)
        {
            ResetDoors(); 
            ResetTriggers();
            ResetCheckPoints();
        }
        else
        {
            OnPlayerRespawn.Invoke();
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

        playerSpawn.playerSpawned.weaponHolder.SwitchToWeaponWithAmmo();
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
        TriggerDoor[] doors = FindObjectsByType<TriggerDoor>(FindObjectsSortMode.None);
        foreach (TriggerDoor door in doors)
        {
            door.Reset();
        }
    }

    public void ResetTriggers()
    {
        TriggerZone[] triggerZones = FindObjectsByType<TriggerZone>(FindObjectsSortMode.None);
        foreach (TriggerZone triggerZone in triggerZones)
        {
            triggerZone.Reset();
        }
    }
    
    public void ResetCheckPoints()
    {
        TriggerCheckPoint[] checkPoints = FindObjectsByType<TriggerCheckPoint>(FindObjectsSortMode.None);
        foreach (TriggerCheckPoint checkPoint in checkPoints)
        {
            checkPoint.Reset();
        }
    }
}