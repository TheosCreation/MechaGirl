
using System;
using UnityEngine;

public class TutorialTextZone : TriggerZone
{
    [TextArea]
    string textToDisplay = "";

    private void Start()
    {
        reuseable = true;
        onTriggerEnter.AddListener(DisplayText);
        onTriggerExit.AddListener(RemoveText);
    }

    private void RemoveText()
    {
        UiManager.Instance.SetTutorialText("");
    }

    private void DisplayText()
    {
        UiManager.Instance.SetTutorialText(textToDisplay);
    }
}