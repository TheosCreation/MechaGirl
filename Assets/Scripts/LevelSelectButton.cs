using HeathenEngineering.DEMO;
using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelToOpen = 1;
    private Button button;
    [SerializeField] private Image levelLockImage;
    [SerializeField] private TMP_Text score;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField]
    private Transform recordRoot;
    [SerializeField]
    private GameObject recordTemplate;

    private List<GameObject> records = new List<GameObject>();

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GameManager.Instance.TryOpenLevel(levelToOpen-1));

        if (GameManager.Instance.GameState.IsCurrentLevelLocked(levelToOpen-1))
        {
            levelLockImage.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (SteamSettings.Initialized)
        {
            leaderboardManager.UploadScore(100, new int[] { 123 });
            leaderboardManager.GetAllFriendsEntries();
        }

    }
    public void UserScoreUpdated(LeaderboardEntry entry)
    {
        if (entry == null)
            score.text = "Entry null";
        else if (entry.details == null)
            score.text = "Details null";
        else
        {
            // Get total seconds from the entry score
            int totalSeconds = entry.Score;

            // Calculate minutes and seconds
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            // Assuming entry.details[0] contains the milliseconds (as an integer)
            int milliseconds = entry.details.Length > 0 ? Mathf.FloorToInt(entry.details[0]) : 0;

            // Format the time as mm:ss.mmm
            string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);

            // Set the formatted time as the score text
            score.text = formattedTime;
        }
    }

    public void HandleBoardQuery(LeaderboardEntry[] entries)
    {
        while (records.Count > 0)
        {
            var record = records[0];
            records.RemoveAt(0);
            Destroy(record);
        }

        foreach (var entry in entries)
        {
            var go = Instantiate(recordTemplate, recordRoot);

            records.Add(go);

            var comp = go.GetComponent<ExampleLdrboardDisplayRecord>();
            comp.SetEntry(entry);
        }
    }
}
