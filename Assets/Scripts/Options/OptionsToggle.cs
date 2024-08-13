using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsToggle : OptionsBase
{
    private Toggle toggle;
    private Button resetButton;
    private Action<bool> updateValueAction;
    private string playerPrefKey;
    private bool defaultValue;

    public OptionsToggle(Toggle toggle, Button resetButton, Action<bool> updateValueAction, string playerPrefKey, bool defaultValue)
    {
        this.toggle = toggle;
        this.resetButton = resetButton;
        this.updateValueAction = updateValueAction;
        this.playerPrefKey = playerPrefKey;
        this.defaultValue = defaultValue;
    }

    public override void Initialize()
    {
        if (toggle != null)
        {
            bool value = GetPlayerPrefValue();
            toggle.isOn = value;
            updateValueAction(value);

            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetToDefault);
        }
    }

    public override void CleanUp()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetToDefault);
        }
    }

    private void OnToggleValueChanged(bool value)
    {
        SaveValue(value);
        updateValueAction(value);
    }

    private void SaveValue(bool value)
    {
        PlayerPrefs.SetInt(playerPrefKey, value ? 1 : 0);

        // Add more types as needed
        PlayerPrefs.Save();
    }

    private bool GetPlayerPrefValue()
    {
        return PlayerPrefs.GetInt(playerPrefKey, defaultValue ? 1 : 0) == 1;
    }

    public void ResetToDefault()
    {
        if (toggle != null)
        {
            toggle.isOn = defaultValue;
            SaveValue(defaultValue);
            updateValueAction(defaultValue);
        }
    }
}
