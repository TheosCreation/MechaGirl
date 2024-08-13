using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsDropdown : OptionsBase
{
    private TMP_Dropdown dropdown;
    private Button resetButton;
    private Action<int> updateValueAction;
    private string playerPrefKey;
    private int defaultValue;

    public OptionsDropdown(TMP_Dropdown dropdown, Button resetButton, Action<int> updateValueAction, string playerPrefKey, int defaultValue)
    {
        this.dropdown = dropdown;
        this.resetButton = resetButton;
        this.updateValueAction = updateValueAction;
        this.playerPrefKey = playerPrefKey;
        this.defaultValue = defaultValue;
    }

    public override void Initialize()
    {
        if (dropdown != null)
        {
            int value = GetPlayerPrefValue();
            dropdown.value = value;
            updateValueAction(value);

            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetToDefault);
        }
    }

    public override void CleanUp()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetToDefault);
        }
    }

    private void OnDropdownValueChanged(int value)
    {
        SaveValue(value);
        updateValueAction(value);
    }

    private void SaveValue(int value)
    {
        PlayerPrefs.SetInt(playerPrefKey, value);
        PlayerPrefs.Save();
    }

    private int GetPlayerPrefValue()
    {
        return PlayerPrefs.GetInt(playerPrefKey, defaultValue);
    }

    public void ResetToDefault()
    {
        dropdown.value = defaultValue;
        SaveValue(defaultValue);
        updateValueAction(defaultValue);
    }
}
