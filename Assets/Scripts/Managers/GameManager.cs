using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string sceneToLoadFirst = "GameScene";
    public string mainMenuScene = "MainMenu";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoadFirst);
    }

    public void ExitToMainMenu()
    {
        PauseManager.Instance.SetPaused(false);
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
