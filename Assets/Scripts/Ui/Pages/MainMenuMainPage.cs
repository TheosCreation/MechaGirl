public class MainMenuMainPage : UiPage
{
    public void OpenEndlessMode()
    {

    }

    public void OpenCampaignPage()
    {
        MainMenu.Instance.OpenCampaignPage();
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