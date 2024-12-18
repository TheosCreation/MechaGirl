using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField] private bool isPaused = false;
    public bool canUnpause = true;

    private void Start()
    {
        CheckPaused();
    }

    private void CheckPaused()
    {
        if (!canUnpause) return;

        if (isPaused)
        {
            Pause();
        }
        else
        {
            UnPause();
        }
    }

    public void SetPaused(bool pausedStatus)
    {
        isPaused = pausedStatus;
        CheckPaused();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        CheckPaused();
    }

    private void Pause()
    {
        InputManager.Instance.DisableInGameInput();
        if (UiManager.Instance != null)
        {
            UiManager.Instance.PauseMenu(true);
        }
        Time.timeScale = 0;


        PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
        if (player != null)
        {
            player.SetAudio(false);
        }


        Cursor.lockState = CursorLockMode.None;
    }

    public void PauseNoScreen()
    {
        InputManager.Instance.DisableInGameInput(); 
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }

    private void UnPause()
    {
        Time.timeScale = 1;
        InputManager.Instance.EnableInGameInput();
        UiManager.Instance.PauseMenu(false);
        if (DiscordManager.Instance != null)
        {
            DiscordManager.Instance.ChangeActivity(GameManager.Instance.currentGamemode, GameManager.Instance.GameState.currentLevelIndex);
        }

        PlayerController player = LevelManager.Instance.playerSpawn.playerSpawned;
        if (player != null)
        {
            player.SetAudio(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }
}