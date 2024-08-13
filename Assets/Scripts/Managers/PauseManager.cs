using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckPaused();
    }

    private void CheckPaused()
    {
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
        UiManager.Instance.PauseMenu(true);
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
    }

    private void UnPause()
    {
        Time.timeScale = 1;
        InputManager.Instance.EnableInGameInput();
        UiManager.Instance.PauseMenu(false);


        Cursor.lockState = CursorLockMode.Locked;
    }
}