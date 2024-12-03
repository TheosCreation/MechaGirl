using HeathenEngineering.DEMO;
using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelToOpen = 1;
    [SerializeField]
    private TMP_Text scoreLabel;
    private Button button;
    [SerializeField] private Image levelLockImage;
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
            leaderboardManager.RefreshUserEntry();
            leaderboardManager.GetAllFriendsEntries();
        }

    }

    public void UserScoreUpdated(LeaderboardEntry entry)
    {
        if (entry == null)
            scoreLabel.text = "Score: NA\nRank: NA\nDetails: NULL";
        else if (entry.details == null)
            scoreLabel.text = "Score: " + entry.Score.ToString() + "\nRank: " + entry.Rank.ToString() + "\nDetails: NULL";
        else
        {
            string details = "{ ";
            for (int i = 0; i < entry.details.Length; i++)
            {
                if (i == 0)
                    details += entry.details[i].ToString();
                else
                    details += ", " + entry.details[i].ToString();
            }

            details += " }";
            scoreLabel.text = "Score: " + entry.Score.ToString() + "\nRank: " + entry.Rank.ToString() + "\nDetails: " + details;
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
