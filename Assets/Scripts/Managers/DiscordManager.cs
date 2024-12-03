using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiscordManager : SingletonPersistent<DiscordManager>
{
    Discord.Discord m_discord;
    const long discordApplicationId = 1313271535297101855; 

    public IEnumerator Init()
    {
        m_discord = new Discord.Discord(discordApplicationId, (ulong)Discord.CreateFlags.NoRequireDiscord);

        yield return null;

        Debug.Log("Discord initialization complete.");
        ChangeActivity();
    }

    private void OnDisable()
    {
        m_discord.Dispose();
    }

    public void ChangeActivity()
    {
        var activityManger = m_discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = "Campaign",
            Details = "Level 1",
            Timestamps = new Discord.ActivityTimestamps
            {
                Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            },
            Party = new Discord.ActivityParty
            {
                Id = "party123"
            }
        };
        activityManger.UpdateActivity(activity, (res) =>
        {
            Debug.Log("Activity updated!");
        });
    }

    private void Update()
    {
        if (m_discord != null)
        {
            m_discord.RunCallbacks();
        }
    }
}
