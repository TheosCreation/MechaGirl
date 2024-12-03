using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Singleton<MainMenu>, IMenuManager
{
    [SerializeField] private MainMenuMainPage mainPage;
    [SerializeField] private OptionsMenu optionsPage;
    [SerializeField] private DifficultySelectPage difficultySelectPage;
    [SerializeField] private LevelSelectPage levelSelectPage;

    public PlayerInput Input;

    private readonly Stack<UiPage> navigationHistory = new Stack<UiPage>();

    protected override void Awake()
    {
        base.Awake();
        Input = new PlayerInput();
        Input.Ui.Exit.started += _ctx => Back();
        OpenMainPage();
    }

    private void OnEnable()
    {
        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    private void ActivatePage(UiPage page)
    {
        // Deactivate all pages first
        mainPage.SetActive(false);
        optionsPage.SetActive(false);
        difficultySelectPage.SetActive(false);
        levelSelectPage.SetActive(false);

        // Finally, activate the selected page
        page.SetActive(true);
    }


    public void OpenMainPage()
    {
        // Check if the page is already in the stack
        if (navigationHistory.Contains(mainPage)) return;

        // Push to stack and activate it
        navigationHistory.Push(mainPage);
        ActivatePage(mainPage);
    }

    public void OpenOptionsPage()
    {
        if (optionsPage.gameObject.activeSelf) return; // Prevent duplicate pushes
        navigationHistory.Push(optionsPage);
        ActivatePage(optionsPage);
    }

    public void OpenLevelSelectPage()
    {
        if (levelSelectPage.gameObject.activeSelf) return; // Prevent duplicate pushes
        navigationHistory.Push(levelSelectPage);
        ActivatePage(levelSelectPage);
    }

    public void OpenDifficultyPage(Gamemode gamemode)
    {
        if (difficultySelectPage.gameObject.activeSelf) return; // Prevent duplicate pushes

        GameManager.Instance.currentGamemode = gamemode;

        // Ensure the main page is pushed before switching to the difficulty page
        if (!navigationHistory.Contains(mainPage))
        {
            navigationHistory.Push(mainPage);
        }

        navigationHistory.Push(difficultySelectPage);
        ActivatePage(difficultySelectPage);
    }


    public void Quit()
    {
        GameManager.Instance.Quit();
    }

    public void Back()
    {
        if (navigationHistory.Count > 1) // Ensure there's a page to go back to
        {
            // Pop the current page and activate the previous one
            navigationHistory.Pop();
            var previousPage = navigationHistory.Peek();
            ActivatePage(previousPage);
        }
    }
}