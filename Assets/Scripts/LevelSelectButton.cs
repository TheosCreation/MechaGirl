using HeathenEngineering.DEMO;
using HeathenEngineering.SteamworksIntegration;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Decouple the leaderboards from the buttons and make a leaderboard manager that holds all the level entries so we fast track the delay or getting entries
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
        SpawnEntries();
    }

    private void SpawnEntries()
    {
        LeaderboardEntry[] entries = SteamManager.Instance.GetEntries(levelToOpen);

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

            var comp = go.GetComponent<LeaderboardRecord>();
            comp.SetEntry(entry);
        }
    }
}
