using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Singleton<MainMenu>, IMenuManager
{
    [SerializeField] private MainMenuMainPage mainPage;
    [SerializeField] private OptionsMenu optionsPage;
    [SerializeField] private DifficultySelectPage difficultySelectPage;
    [SerializeField] private GameObject levelSelectPage;

    public PlayerInput Input;

    private readonly Stack<GameObject> navigationHistory = new Stack<GameObject>();

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

    private void ActivatePage(GameObject page)
    {
        mainPage.gameObject.SetActive(false);
        optionsPage.gameObject.SetActive(false);
        difficultySelectPage.gameObject.SetActive(false);
        levelSelectPage.SetActive(false);

        page.SetActive(true);
    }


    public void OpenMainPage()
    {
        // Check if the page is already in the stack
        if (navigationHistory.Contains(mainPage.gameObject)) return;

        // Push to stack and activate it
        navigationHistory.Push(mainPage.gameObject);
        ActivatePage(mainPage.gameObject);
    }

    public void OpenOptionsPage()
    {
        if (optionsPage.gameObject.activeSelf) return; // Prevent duplicate pushes
        navigationHistory.Push(optionsPage.gameObject);
        ActivatePage(optionsPage.gameObject);
    }

    public void OpenLevelSelectPage()
    {
        if (levelSelectPage.activeSelf) return; // Prevent duplicate pushes
        navigationHistory.Push(levelSelectPage);
        ActivatePage(levelSelectPage);
    }

    public void OpenCampaignPage()
    {
        if (difficultySelectPage.gameObject.activeSelf) return; // Prevent duplicate pushes

        // Ensure the main page is pushed before switching to the difficulty page
        if (!navigationHistory.Contains(mainPage.gameObject))
        {
            navigationHistory.Push(mainPage.gameObject);
        }

        navigationHistory.Push(difficultySelectPage.gameObject);
        ActivatePage(difficultySelectPage.gameObject);
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