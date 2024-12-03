using UnityEngine;

public class DifficultySelectPage : UiPage
{
    protected override void OnEnable()
    {
        base.OnEnable();

        if(DiscordManager.Instance)
        {
            DiscordManager.Instance.ChangeActivity(Gamemode.Menu, 0, "Difficulty Select Menu");
        }
    }
    public void SelectDifficulty(int difficultyLevel = 0)
    {
        GameManager.Instance.SetDifficultyLevel(difficultyLevel);

        if (GameManager.Instance.currentGamemode == Gamemode.Campaign)
        {
            MainMenu.Instance.OpenLevelSelectPage();
        }
        else if (GameManager.Instance.currentGamemode == Gamemode.Endless)
        {
            GameManager.Instance.OpenEndlessModeLevel();
        }
        else
        {
            Debug.LogError("Issue current gamemode is not set right cannot continue");
        }
    }
}