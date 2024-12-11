using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// This is for demonstration purposes only
/// </summary>
public class LeaderboardRecord : MonoBehaviour
{
    public SetUserAvatar userImage;
    public UGUISetUserName userName;
    public TMP_Text score;
    public TMP_Text difficulty;

    public void SetEntry(LeaderboardEntry entry)
    {
        if (entry == null)
        {
            Debug.Log("Entry null");
            return;
        }

        userImage.LoadAvatar(entry.User);
        userName.SetName(entry.User);

        Debug.Log(entry.Score);

        int minutes = entry.Score / 60000; // Calculate minutes
        int seconds = (entry.Score / 1000) % 60; // Calculate remaining seconds
        int milliseconds = entry.Score % 1000; // Calculate remaining milliseconds

        string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);

        score.text = formattedTime;

        //difficulty.text = "";
    }
}