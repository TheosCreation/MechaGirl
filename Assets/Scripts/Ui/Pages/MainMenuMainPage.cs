public class MainMenuMainPage : UiPage
{
    public void OpenLevelSelectPage()
    {
        MainMenu.Instance.OpenLevelSelectPage();
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