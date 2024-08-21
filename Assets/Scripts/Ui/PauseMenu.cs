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

    public void ResetLevel()
    {
        LevelManager.Instance.resetLevel = true;
        LevelManager.Instance.KillCurrentPlayer();
        PauseManager.Instance.SetPaused(true);
        LevelManager.Instance.RespawnPlayer();
    }

    public void ResetToCheckPoint()
    {
        LevelManager.Instance.KillCurrentPlayer();
        PauseManager.Instance.SetPaused(true);
        LevelManager.Instance.RespawnPlayer();
    }
}
