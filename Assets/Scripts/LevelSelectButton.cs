using HeathenEngineering.DEMO;
using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelToOpen = 1;
    [SerializeField] private string levelName = "Hell";
    private Button button;
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private Image levelIcon;
    [SerializeField] private Image levelLockImage;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField]
    private Transform recordRoot;
    [SerializeField] private GameObject leaderBoardRoot;
    [SerializeField]
    private GameObject recordTemplate;

    private List<GameObject> records = new List<GameObject>();

    [SerializeField] private Sprite blurredImage;
    private Sprite orignalImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { GameManager.Instance.TryOpenLevel(levelToOpen - 1); MainMenu.Instance.OpenLoadingScreen(); });
        orignalImage = levelIcon.sprite;

        if (GameManager.Instance.GameState.IsCurrentLevelLocked(levelToOpen-1))
        {
            titleLabel.text = levelToOpen + ": - ???";
            levelLockImage.gameObject.SetActive(true);
            leaderBoardRoot.SetActive(false);
            levelIcon.sprite = blurredImage;
        }
        else
        {
            titleLabel.text = levelToOpen + ": - " + levelName;
        }
    }

    private void OnEnable()
    {
        if (SteamSettings.Initialized)
        {
            int encodedScore = (2 * 1000) + 150; // 3 seconds and 150 milliseconds
            leaderboardManager.UploadScore(encodedScore);
            leaderboardManager.GetAllFriendsEntries();
        }
    }

    public void UserScoreUpdated(LeaderboardEntry entry)
    {
        if (entry != null)
        {
            if (entry.details == null) Debug.LogWarning("details null");
            // Get total seconds from the entry score
            int totalSeconds = entry.Score;

            // Calculate minutes and seconds
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            // Assuming entry.details[0] contains the milliseconds (as an integer)
            //int milliseconds = entry.details.Length > 0 ? Mathf.FloorToInt(entry.details[0]) : 0;

            // Format the time as mm:ss.mmm
            string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            //string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);

            // Set the formatted time as the score text
            //score.text = formattedTime;
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
            go.SetActive(true);

            records.Add(go);

            var comp = go.GetComponent<ExampleLdrboardDisplayRecord>();
            comp.SetEntry(entry);
        }
    }

    public void LogError(string message)
    {
        Debug.LogError(message);
    }
}
