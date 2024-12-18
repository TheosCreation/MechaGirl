using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour, IMenuManager
{
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Button checkPointButton;

    private void OnEnable()
    {
        checkPointButton.onClick.AddListener(LevelManager.Instance.ResetToCheckPoint);
        OpenMainPage();
    }

    private void OnDisable()
    {
        checkPointButton.onClick.RemoveAllListeners();
    }

    public void OpenMainPage()
    {
        mainPage.SetActive(true);
        optionsMenu.SetActive(false);


        if (DiscordManager.Instance)
        {
            DiscordManager.Instance.ChangeActivity(Gamemode.Menu, 0, "Pause Menu");
        }
    }
    
    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
        mainPage.SetActive(false);
    }

    public void ResumeGame()
    {
        PauseManager.Instance.SetPaused(false);
    }

    public void ExitToMainMenu()
    {
        GameManager.Instance.ExitToMainMenu();
    }

    public void ResetLevel()
    {
        GameManager.Instance.ReopenLevel();
    }

    public void Back()
    {
        OpenMainPage();
    }
}
