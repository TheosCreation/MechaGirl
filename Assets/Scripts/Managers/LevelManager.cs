using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct WeaponSpawn
{
    public Weapon weaponPrefab;
    public int startingAmmo;
}

class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private float levelStartTime;
    private float levelCompleteTime;
    private bool isTimerRunning;
    [HideInInspector] public PlayerSpawn playerSpawn;
    [SerializeField] public GameObject tempCamera;
    [HideInInspector] public UnityEvent OnPlayerRespawn;
    private TriggerCheckPoint currentCheckPoint;

    public List<Keycard> currentHeldKeycards;
    [SerializeField] List<WeaponSpawn> weaponsToSpawnWith;

    private List<IResetable> resetables = new List<IResetable>();

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

        playerSpawn = FindFirstObjectByType<PlayerSpawn>();
        if (playerSpawn == null)
        {
            Debug.LogAssertion("Player Spawn does not exist in scene cannot continue play");
        }

        playerSpawn.SpawnPlayer(weaponsToSpawnWith);
    }

    void Start()
    {
        if(!GameManager.Instance.isInit)
        {
            GameManager.Instance.Init();
        }

        if (DiscordManager.Instance != null)
        {
            DiscordManager.Instance.ChangeActivity(GameManager.Instance.currentGamemode, GameManager.Instance.GameState.currentLevelIndex);
        }
    }

    public void StartLevelTimer()
    {
        // Start the timer
        levelStartTime = Time.time;
        isTimerRunning = true;
    }
    private void FixedUpdate()
    {
        if (isTimerRunning)
        {
            float elapsedTime = Time.time - levelStartTime;

            int seconds = Mathf.FloorToInt(elapsedTime); // Total elapsed seconds
            int milliseconds = Mathf.FloorToInt((elapsedTime - seconds) * 1000); // Milliseconds part

            UiManager.Instance.playerHud.UpdateLevelTimeText(seconds, milliseconds);
        }
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
            }
            int currentLevel = GameManager.Instance.GameState.currentLevelIndex;
            GameManager.Instance.GameState.UnlockLevel(currentLevel + 1);
            int seconds = (int)Mathf.Floor(bestTimeForCurrentLevel);
            int milliseconds = (int)((bestTimeForCurrentLevel - seconds) * 1000);
            SteamManager.Instance.UploadLevelTime(currentLevel + 1, seconds, milliseconds);

            // Pass the time to the UIManager and open the level complete screen
            UiManager.Instance.OpenLevelCompleteScreen(levelCompleteTime, currentLevel + 1);
        }
    }

    // Use only if the player is going to be respawned in the next frame
    public void KillCurrentPlayer()
    {
        if (playerSpawn.playerSpawned)
        {
            Destroy(playerSpawn.playerSpawned.gameObject);
            SetTempCamera(true);
        }
    }
    public void ResetToCheckPoint()
    {
        KillCurrentPlayer();
        DestroyAllEnemies();
        DestroyAllWeapons();

        StartCoroutine(RespawnPlayerNextFrame());
    }

    private IEnumerator RespawnPlayerNextFrame()
    {
        yield return new WaitForEndOfFrame(); // Waits until the end of the current frame
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        // Spawn the new player with the weapons again
        playerSpawn.SpawnPlayer(weaponsToSpawnWith);

        ResetLevel();
        OnPlayerRespawn?.Invoke();

        UiManager.Instance.OpenPlayerHud();

        SetTempCamera(false);
        PauseManager.Instance.SetPaused(false);
        SettingsManager.Instance.ApplyAllSettings();

        //playerSpawn.playerSpawned.weaponHolder.SwitchToWeaponWithAmmo();
    }

    public void SetCheckPoint(TriggerCheckPoint checkPoint)
    {
        playerSpawn.transform.position = checkPoint.transform.position;
        playerSpawn.transform.rotation = checkPoint.transform.rotation;

        currentCheckPoint = checkPoint;
        foreach (var weaponSpawner in currentCheckPoint.weaponSpawners)
        {
            weaponsToSpawnWith.Add(weaponSpawner.weaponToSpawn);
        }
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

    public void RegisterObject(IResetable resetable)
    {
        if (!resetables.Contains(resetable))
            resetables.Add(resetable);
    }
    public void Deregister(IResetable resetable)
    {
        if (resetables.Contains(resetable))
            resetables.Remove(resetable);
    }

    public void ResetLevel()
    {
        foreach (IResetable resetable in resetables)
        {
            resetable.Reset();
        }
    }

    public void SetTempCamera(bool active)
    {
        tempCamera.SetActive(active);
    }
}