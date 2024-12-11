using TMPro;
using UnityEngine;

public class PlayerHud : UiPage
{

    [Header("Speedrun Ui")]
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text levelTime;

    protected override void OnEnable()
    {
        base.OnEnable();

        ToggleSpeedrunMode(SettingsManager.Instance.speedrunMode);
    }

    public void ToggleSpeedrunMode(bool speedrunMode)
    {
        if (speedText != null)
        {
            speedText.gameObject.SetActive(speedrunMode);
        }
        if (levelTime != null)
        {
            levelTime.gameObject.SetActive(speedrunMode);
        }
    }

    public void UpdateSpeedText(float speed)
    {
        speedText.text = speed.ToString("F2");
    }

    public void UpdateLevelTimeText(int seconds, int milliseconds)
    {
        int minutes = seconds / 60;
        seconds %= 60; // Remaining seconds after minutes are calculated
        levelTime.text = $"{minutes}:{seconds:00}.{milliseconds:000}";
    }
}