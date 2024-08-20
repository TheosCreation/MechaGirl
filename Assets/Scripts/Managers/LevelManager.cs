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
        OnPlayerRespawn?.Invoke();
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
        //FindObjectsByType<TriggerDoor>()
        SettingsManager.Instance.player = playerSpawn.playerSpawned;

        if (tempCamera != null)
        {
            Destroy(tempCamera);
        }
        PauseManager.Instance.SetPaused(false);
        SettingsManager.Instance.ApplyAllSettings();
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
}