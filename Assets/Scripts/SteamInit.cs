using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;

public class SteamManager : SingletonPersistent<SteamManager>
{
    [SerializeField]
    private SteamSettings settings;
    [HideInInspector] public LeaderboardManager leaderboardManager;

    protected override void Awake()
    {
        base.Awake();
        leaderboardManager = GetComponent<LeaderboardManager>();
    }

    public IEnumerator InitSteam()
    {
        if(settings == null)
        {
            Debug.LogError("Steam settings is null in the inspector");
        }

        settings.CreateBehaviour(true);

        Debug.Log("Initializing Steam...");
        settings.Init();

        yield return null;

        Debug.Log("Steam initialized successfully.");
    }
}