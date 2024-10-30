using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private float levelStartTime;
    private float levelCompleteTime;
    private bool isTimerRunning;
    [HideInInspector] public PlayerSpawn playerSpawn;
    [SerializeField] public GameObject tempCamera;
    [HideInInspector] public UnityEvent OnPlayerRespawn;

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
    }

    void Start()
    {
        if(!GameManager.Instance.isInit)
        {
            GameManager.Instance.Init();
        }
        
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
        ResetLevel();
        OnPlayerRespawn?.Invoke();

        UiManager.Instance.OpenPlayerHud();

        //reset player health reset scene

        playerSpawn.SpawnPlayer();

        //reset doors, remove enemies, reset trigger zones
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
        playerSpawn.transform.position = checkPointTransform.position;
        playerSpawn.transform.rotation = checkPointTransform.rotation;
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

    public void SetTempCamera(GameObject gameObject)
    {
        if (gameObject == null) return;
        tempCamera = gameObject;
        tempCamera.transform.SetParent(null);
    }
}