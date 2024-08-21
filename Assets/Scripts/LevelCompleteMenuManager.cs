using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelCompleteTitle;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private Button button;

    private void OnEnable()
    {
        SetComponents(false);

        Timer timer = gameObject.AddComponent<Timer>();
        timer.SetTimer(0.5f, SetComponents, true);
    }

    private void SetComponents(bool enabled)
    {
        levelCompleteTitle.gameObject.SetActive(enabled);
        timeText.gameObject.SetActive(enabled);
        bestTimeText.gameObject.SetActive(enabled);
        button.gameObject.SetActive(enabled);
    }
    public void UpdateBestTimeText(float time)
    {
        // Format the time into minutes:seconds.milliseconds
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        float milliseconds = time % 1 * 1000;

        bestTimeText.text = string.Format("{0}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void UpdateTimeText(float time)
    {
        // Format the time into minutes:seconds.milliseconds
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        float milliseconds = time % 1 * 1000;

        timeText.text = string.Format("{0}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void UpdateLevelNumber(int levelNumber)
    {
        // Example: "Level 1 Complete"
        levelCompleteTitle.text = $"Level {levelNumber} Complete";
    }

    public void OpenNextLevel()
    {
        GameManager.Instance.OpenNextLevel();
    }
}