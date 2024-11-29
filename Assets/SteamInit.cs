using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using System.Collections;

public class SteamInit : MonoBehaviour
{
    [SerializeField]
    private SteamSettings settings;

    public IEnumerator InitSteam()
    {
        if(settings == null)
        {
            Debug.LogError("Steam settings is null in the inspector");
        }

        Debug.Log("Initializing Steam...");
        settings.Init();

        yield return null;

        Debug.Log("Steam initialized successfully.");
    }
}