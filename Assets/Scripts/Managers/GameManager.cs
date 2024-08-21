using Assets.Scripts.JsonSerialization;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public GameState GameState;
    public string mainMenuScene = "MainMenu";
    private string[] levelScenes;

    private IDataService DataService = new JsonDataService();
    private long SaveTime;
    private long LoadTime;

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

            GameState = new GameState(levelScenes.Length);
        }
        else
        {
            Destroy(gameObject);
        }

        UnSerializeGameStateFromJson();
    }

    public void UnSerializeGameStateFromJson()
    {
        long startTime = DateTime.Now.Ticks;
        try
        {
            GameState data = DataService.LoadData<GameState>("/game-state.json", false);
            LoadTime = DateTime.Now.Ticks - startTime;
            Debug.Log("Load Time: " + LoadTime);
        }
        catch
        {
            Debug.Log("Game state file does not exist, fresh start");
        }
    }

    public void SerializeGameStateToJson()
    {
        long startTime = DateTime.Now.Ticks;
        if(DataService.SaveData("/game-state.json", GameState, false))
        {
            SaveTime = DateTime.Now.Ticks - startTime;
            Debug.Log("Save Time: " +  SaveTime);
        }
        else
        {
            Debug.LogError("Could not save file!");
        }
    }

    public void StartGame()
    {
        if (levelScenes.Length > 0)
        {
            GameState.currentLevelIndex = 0;
            SceneManager.LoadScene(levelScenes[GameState.currentLevelIndex]);
        }
        else
        {
            Debug.LogError("No levels found that start with 'Level'.");
        }
    }

    public void OpenNextLevel()
    {
        if (GameState.currentLevelIndex < levelScenes.Length - 1)
        {
            GameState.currentLevelIndex++;
            SceneManager.LoadScene(levelScenes[GameState.currentLevelIndex]);
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