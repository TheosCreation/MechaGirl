public class DifficultySelectPage : UiPage
{ 
    public void SelectDifficulty(int difficultyLevel = 0)
    {
        GameManager.Instance.SetDifficultyLevel(difficultyLevel);
        MainMenu.Instance.OpenLevelSelectPage();
    }
}