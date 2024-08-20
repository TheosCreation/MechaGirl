using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string mainMenuScene = "MainMenu";

    public int currentLevelIndex = 0;
    private string[] levelScenes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Get scenes that start with "Level" and store them
            levelScenes = Enumerable.Range(0, SceneManager.sceneCountInBuildSettings)
                .Select(i => System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)))
                .Where(name => name.StartsWith("Level"))
                .OrderBy(name => name)
                .ToArray();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        if (levelScenes.Length > 0)
        {
            currentLevelIndex = 0;
            SceneManager.LoadScene(levelScenes[currentLevelIndex]);
        }
        else
        {
            Debug.LogError("No levels found that start with 'Level'.");
        }
    }

    public void OpenNextLevel()
    {
        if (currentLevelIndex < levelScenes.Length - 1)
        {
            currentLevelIndex++;
            SceneManager.LoadScene(levelScenes[currentLevelIndex]);
        }
        else
        {
            Debug.Log("No more levels to load, returning to main menu.");
            ExitToMainMenu();
        }
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