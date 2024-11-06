using System.Collections;
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

    public void ResetToCheckPoint()
    {
        LevelManager.Instance.KillCurrentPlayer();
        //PauseManager.Instance.SetPaused(false);

        StartCoroutine(RespawnPlayerNextFrame());
    }

    private IEnumerator RespawnPlayerNextFrame()
    {
        yield return new WaitForEndOfFrame(); // Waits until the end of the current frame
        LevelManager.Instance.RespawnPlayer();
    }
}
