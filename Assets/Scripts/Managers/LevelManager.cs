using UnityEngine;

class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private float levelStartTime;
    private float levelCompleteTime;
    private bool isTimerRunning;
    private Transform respawnTransform;
    private PlayerSpawn playerSpawn;

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

        PauseManager.Instance.SetPaused(false);
    }

    public void SetCheckPoint(Transform checkPointTransform)
    {
        respawnTransform = checkPointTransform;
    }
}