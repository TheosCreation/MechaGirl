using System.Collections.Generic;

public class GameState
{
    public Dictionary<int, float> levelBestTimes = new Dictionary<int, float>();

    public int currentLevelIndex = 0;

    public GameState(int levelCount)
    {
        // Initialize the dictionary with a default value for each level
        for (int i = 0; i < levelCount; i++)
        {
            levelBestTimes[i] = float.MaxValue;
        }
    }

    public float GetBestTimeForCurrentLevel()
    {
        if (levelBestTimes.ContainsKey(currentLevelIndex))
        {
            return levelBestTimes[currentLevelIndex];
        }
        return float.MaxValue;
    }

    public void SetBestTimeForCurrentLevel(float time)
    {
        if (levelBestTimes.ContainsKey(currentLevelIndex))
        {
            levelBestTimes[currentLevelIndex] = time;
        }
    }
}
