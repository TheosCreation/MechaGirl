#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS

using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.UI;
using TMPro;
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class ExampleLdrboardDisplayRecord : MonoBehaviour
    {
        public SetUserAvatar userImage;
        public UGUISetUserName userName;
        public TMP_Text score;
        public TMP_Text difficulty;

        public void SetEntry(LeaderboardEntry entry)
        {
            if (entry == null) return;

            userImage.LoadAvatar(entry.User);
            userName.SetName(entry.User);

            int totalSeconds = entry.Score;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, entry.details[0]);

            score.text = formattedTime;

            //difficulty.text = "";
        }
    }
}
#endif