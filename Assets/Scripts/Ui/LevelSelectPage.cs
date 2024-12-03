
public class LevelSelectPage : UiPage
{
    protected override void OnEnable()
    {
        base.OnEnable();

        if(DiscordManager.Instance)
        {
            DiscordManager.Instance.ChangeActivity(Gamemode.Menu, 0, "Level Select Menu");
        }
    }
}