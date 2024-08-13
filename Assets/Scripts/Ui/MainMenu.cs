using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject optionsPage;

    public PlayerInput Input;

    private void Awake()
    {
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
        mainPage.SetActive(true);
        optionsPage.SetActive(false);
    }

    public void OpenOptionsPage()
    {
        optionsPage.SetActive(true);
        mainPage.SetActive(false);
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void Back()
    {
        if(optionsPage.activeSelf)
        {
            OpenMainPage();
        }
    }
}
