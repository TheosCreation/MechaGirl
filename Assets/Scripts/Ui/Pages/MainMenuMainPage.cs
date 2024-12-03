public class MainMenuMainPage : UiPage
{
    protected override void OnEnable()
    {
        base.OnEnable();

        if (DiscordManager.Instance != null)
        {
            DiscordManager.Instance.ChangeActivity(Gamemode.Menu, 0, "Main Menu");
        }
    }
    public void OpenEndlessMode()
    {
        MainMenu.Instance.OpenDifficultyPage(Gamemode.Endless);
    }

    public void OpenCampaignPage()
    {
        MainMenu.Instance.OpenDifficultyPage(Gamemode.Campaign);
    }

    public void OpenOptionPage()
    {
        MainMenu.Instance.OpenOptionsPage();
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }
}