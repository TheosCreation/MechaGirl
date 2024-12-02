using UnityEngine;
using System.Text;

public class TutorialTextZone : TriggerZone
{
    [TextArea, SerializeField]
    private string textToDisplay = ""; // Text set in the editor

    private string dynamicText; // To store additional dynamic text

    private void Start()
    {
        reuseable = true;
        onTriggerEnter.AddListener(DisplayText);
        onTriggerExit.AddListener(RemoveText);
    }

    private void RemoveText()
    {
        UiManager.Instance.SetTutorialText(""); // Clear the tutorial text
    }

    private void DisplayText()
    {
        // Combine the static text from the editor and dynamic text
        string fullText = dynamicText + textToDisplay;
        UiManager.Instance.SetTutorialText(fullText);
    }

    public void SetDynamicText(string text)
    {
        dynamicText = text;
    }
}