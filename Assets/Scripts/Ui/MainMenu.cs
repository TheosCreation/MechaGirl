using UnityEngine;

public class MainMenu : Singleton<MainMenu>, IMenuManager
{
    [SerializeField] private MainMenuMainPage mainPage;
    [SerializeField] private GameObject optionsPage;
    [SerializeField] private GameObject levelSelectPage;

    public PlayerInput Input;

    protected override void Awake()
    {
        base.Awake();

        Input = new PlayerInput();
    }
    private void OnEnable()
    {
        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }
    public void Start()
    {
        Input.Ui.Exit.started += _ctx => Back();
        OpenMainPage();
    }

    public void OpenMainPage()
    {
        mainPage.gameObject.SetActive(true);
        optionsPage.SetActive(false);
        levelSelectPage.SetActive(false);
    }

    public void OpenOptionsPage()
    {
        optionsPage.SetActive(true);
        mainPage.gameObject.SetActive(false);
    }
    public void OpenLevelSelectPage()
    {
        levelSelectPage.SetActive(true);
        mainPage.gameObject.SetActive(false);
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }

    public void Back()
    {
        if(optionsPage.activeSelf)
        {
            OpenMainPage();
        }
    }
}
