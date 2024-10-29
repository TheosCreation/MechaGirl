using Assets.Scripts.JsonSerialization;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public GameState GameState;
    public string mainMenuScene = "MainMenu";
    private string[] levelScenes;

    private IDataService DataService = new JsonDataService();
    private long SaveTime;
    private long LoadTime;

    private string gameStateFilePath;
    public bool isInit = false;


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

    public IEnumerator Init()
    {
        // Get scenes that start with "Level" and store them
        levelScenes = Enumerable.Range(0, SceneManager.sceneCountInBuildSettings)
            .Select(i => System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)))
            .Where(name => name.StartsWith("Level"))
            .OrderBy(name => name)
            .ToArray();

        gameStateFilePath = "/game-state.json";
        UnSerializeGameStateFromJson();

        yield return null;

        Debug.Log("GameManager initialization complete.");
        isInit = true;
    }

    public void UnSerializeGameStateFromJson()
    {
        long startTime = DateTime.Now.Ticks;
        try
        {
            GameState data = DataService.LoadData<GameState>(gameStateFilePath, false);
            if (data != null)
            {
                GameState = data; // Load the game state if data is present
                Debug.Log("Game state loaded successfully.");
            }
            else
            {
                // If the file doesn't exist or is empty, ensure a fresh start
                GameState = new GameState(levelScenes.Length);
                Debug.Log("No game state data found, starting fresh.");
            }
        }
        catch (Exception ex)
        {
            // Log the error but ensure the game can still proceed
            Debug.LogError("Error loading game state: " + ex.Message);

            // Initialize default game state to ensure gameplay can continue
            GameState = new GameState(levelScenes.Length);
            Debug.Log("Initialized fresh game state to allow the game to proceed.");
        }

        // Record load time for performance tracking
        LoadTime = DateTime.Now.Ticks - startTime;
        Debug.Log("Load Time: " + LoadTime);
    }
    public void SerializeGameStateToJson()
    {
        long startTime = DateTime.Now.Ticks;
        try
        {
            if (DataService.SaveData(gameStateFilePath, GameState, false))
            {
                SaveTime = DateTime.Now.Ticks - startTime;
                Debug.Log("Game state saved successfully. Save Time: " + SaveTime);
            }
            else
            {
                Debug.LogError("Failed to save game state.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving game state: " + ex.Message);
        }
    }

    public void StartGame()
    {
        if (levelScenes.Length > 0)
        {
            GameState.currentLevelIndex = 0;
            SceneManager.LoadSceneAsync(levelScenes[GameState.currentLevelIndex]);
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
            SceneManager.LoadSceneAsync(levelScenes[GameState.currentLevelIndex]); //May required load screen
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

    private void OnApplicationQuit()
    {
        SerializeGameStateToJson();
    }
}