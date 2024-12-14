using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;

public class SteamManager : SingletonPersistent<SteamManager>
{
    [SerializeField]
    private SteamSettings settings;
    public LeaderboardManager[] levelLeaderBoards; // 0 = level 1 leaderboard
    public Dictionary<ulong, LeaderboardEntry[]> leaderboardEntries = new Dictionary<ulong, LeaderboardEntry[]>();

    protected override void Awake()
    {
        base.Awake();
        levelLeaderBoards = GetComponents<LeaderboardManager>();
        foreach (var levelLeaderboard in levelLeaderBoards)
        {
            levelLeaderboard.evtQueryCompleted.AddListener(HandleBoardQuery);
        }
    }

    public IEnumerator InitSteam()
    {
        if(settings == null)
        {
            Debug.LogError("Steam settings is null in the inspector");
        }


        Debug.Log("Initializing Steam...");
        settings.CreateBehaviour(true);

        yield return null;

        GetFriendsLeaderboardEntries();

        yield return null;

        Debug.Log("Steam initialized successfully.");
    }

    public void GetFriendsLeaderboardEntries()
    {
        foreach (var levelLeaderboard in levelLeaderBoards)
        {
            levelLeaderboard.GetAllFriendsEntries();
        }
    }

    public void UploadLevelTime(int level, int seconds, int milliseconds)
    {
        int totalMilliseconds = (seconds * 1000) + milliseconds;
        levelLeaderBoards[level - 1].UploadScore(totalMilliseconds);
        Debug.Log("Uploading score: " + totalMilliseconds);
    }

    public void HandleBoardQuery(ulong leaderboardId, LeaderboardEntry[] entries)
    {
        if (entries != null)
        {
            if (leaderboardEntries.ContainsKey(leaderboardId))
            {
                // Handle existing key: either update, overwrite, or log a message
                Debug.LogWarning($"Leaderboard ID {leaderboardId} already exists. Overwriting entries.");
                leaderboardEntries[leaderboardId] = entries; // Overwrite existing entries
            }
            else
            {
                // Add new entry to the dictionary
                leaderboardEntries.Add(leaderboardId, entries);
            }
        
            Debug.Log($"Updated leaderboard entries for ID {leaderboardId}. Total entries: {entries.Length}");
        }
        else
        {
            Debug.LogWarning($"Received null entries for leaderboard ID {leaderboardId}.");
        }
    }

    public LeaderboardEntry[] GetEntries(int level)
    {
        ulong leaderboardId = levelLeaderBoards[level - 1].leaderboard.leaderboardId.m_SteamLeaderboard;
        return leaderboardEntries[leaderboardId];
    }
}