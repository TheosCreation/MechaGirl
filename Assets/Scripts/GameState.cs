using System.Collections.Generic;

public class GameState
{
    // Dictionary to store the best time for each level
    public Dictionary<int, float> levelBestTimes = new Dictionary<int, float>();

    // Dictionary to store whether each level is locked or unlocked
    public Dictionary<int, bool> levelLocked = new Dictionary<int, bool>();

    public int currentLevelIndex = 0;

    // Constructor to initialize the dictionaries
    public GameState(int levelCount)
    {
        // Initialize the dictionaries with default values for each level
        for (int i = 0; i < levelCount; i++)
        {
            // Set the best time to a high value initially (indicating no time has been set)
            levelBestTimes[i] = float.MaxValue;

            // Set all levels to locked by default, except for the first level
            levelLocked[i] = (i == 0) ? false : true; // Only the first level is unlocked
        }
    }

    // Method to get the best time for the current level
    public float GetBestTimeForCurrentLevel()
    {
        if (levelBestTimes.ContainsKey(currentLevelIndex))
        {
            return levelBestTimes[currentLevelIndex];
        }
        return float.MaxValue;
    }

    // Method to set the best time for the current level
    public void SetBestTimeForCurrentLevel(float time)
    {
        if (levelBestTimes.ContainsKey(currentLevelIndex))
        {
            levelBestTimes[currentLevelIndex] = time;
        }
    }

    // Method to check if the current level is locked
    public bool IsCurrentLevelLocked(int levelIndex)
    {
        if (levelLocked.ContainsKey(levelIndex))
        {
            return levelLocked[levelIndex];
        }
        return false; // If the level doesn't exist, consider it locked
    }

    // Method to unlock a specific level by index
    public void UnlockLevel(int levelIndex)
    {
        if (levelLocked.ContainsKey(levelIndex))
        {
            levelLocked[levelIndex] = false;
        }
    }
}
