using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelCompleteMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    public void UpdateTimeText(float time)
    {
        // Format the time into minutes:seconds.milliseconds
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        float milliseconds = time % 1 * 1000;

        timeText.text = string.Format("{0}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}