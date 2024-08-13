using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject optionsMenu;

    private void OnEnable()
    {
        OpenMainPage();
    }

    public void OpenMainPage()
    {
        mainPage.SetActive(true);
        optionsMenu.SetActive(false);
    }
    
    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
        mainPage.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        GameManager.Instance.ExitToMainMenu();
    }
}
