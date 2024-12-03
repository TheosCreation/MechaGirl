using System;
using System.Collections;
using UnityEngine;

public enum Gamemode
{
    Campaign,
    Endless,
    Menu
}

public class DiscordManager : SingletonPersistent<DiscordManager>
{
    Discord.Discord m_discord;
    const long discordApplicationId = 1313271535297101855; 

    public IEnumerator Init()
    {
        m_discord = new Discord.Discord(discordApplicationId, (ulong)Discord.CreateFlags.NoRequireDiscord);

        yield return null;

        Debug.Log("Discord initialization complete.");
        ChangeActivity(Gamemode.Menu, 0, "Main Menu");
    }

    private void OnDisable()
    {
        m_discord.Dispose();
    }

    public void ChangeActivity(Gamemode gameMode, int? level = null, string menuName = null)
    {
        if (m_discord == null)
        {
            Debug.LogWarning("Discord is not initialized.");
            return;
        }

        var activityManager = m_discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            Timestamps = new Discord.ActivityTimestamps
            {
                Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            },
            Party = new Discord.ActivityParty
            {
                Id = "party123"
            }
        };

        // Customize activity based on the game mode
        if (gameMode == Gamemode.Campaign)
        {
            activity.State = "Campaign";
            activity.Details = level.HasValue ? $"Level {level + 1}" : "Exploring";
        }
        else if (gameMode == Gamemode.Endless)
        {
            activity.State = "Endless Mode";
            activity.Details = "Surviving the waves";
        }
        else if (gameMode == Gamemode.Menu)
        {
            activity.State = "In Menu";
            activity.Details = menuName != null ? $"Browsing {menuName}" : "Browsing the menu";
        }
        else
        {
            activity.State = "In Game";
            activity.Details = "Exploring the unknown";
        }

        // Update the Discord activity
        activityManager.UpdateActivity(activity, (result) =>
        {
            if (result != Discord.Result.Ok)
            {
                Debug.LogError($"Failed to update Discord activity: {result}");
            }
        });
    }

    private void Update()
    {
        m_discord?.RunCallbacks();
    }
}
