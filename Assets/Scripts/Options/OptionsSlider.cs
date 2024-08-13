using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSlider<T> : OptionsBase
{
    private Slider slider;
    private TMP_Text text;
    private Button resetButton;
    private Action<T> updateValueAction;
    private string playerPrefKey;
    private T defaultValue;
    private bool isPercentage;

    public OptionsSlider(Slider slider, TMP_Text text, Button resetButton, Action<T> updateValueAction, string playerPrefKey, T defaultValue, bool isPercentage)
    {
        this.slider = slider;
        this.text = text;
        this.resetButton = resetButton;
        this.updateValueAction = updateValueAction;
        this.playerPrefKey = playerPrefKey;
        this.defaultValue = defaultValue;
        this.isPercentage = isPercentage;
    }

    public override void Initialize()
    {
        if (slider != null)
        {
            T value = GetPlayerPrefValue();
            slider.value = Convert.ToSingle(value);
            UpdateText(value);
            updateValueAction(value);

            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if(resetButton != null)
        {
            resetButton.onClick.AddListener(ResetToDefault);
        }
    }

    public override void CleanUp()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetToDefault);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        T typedValue = (T)Convert.ChangeType(value, typeof(T));
        UpdateText(typedValue);
        SaveValue(typedValue);
        updateValueAction(typedValue);
    }

    private void UpdateText(T value)
    {
        string settingsText = value.ToString();
        if(isPercentage)
        {
            settingsText += "%";
        }
        text.text = settingsText;
    }

    private void SaveValue(T value)
    {
        if (typeof(T) == typeof(float))
        {
            PlayerPrefs.SetFloat(playerPrefKey, Convert.ToSingle(value));
        }
        // Add more types as needed
        PlayerPrefs.Save();
    }

    private T GetPlayerPrefValue()
    {
        if (typeof(T) == typeof(float))
        {
            return (T)(object)PlayerPrefs.GetFloat(playerPrefKey, Convert.ToSingle(defaultValue));
        }
        // Add more types as needed
        return defaultValue;
    }

    public void ResetToDefault()
    {
        slider.value = Convert.ToSingle(defaultValue);
        UpdateText(defaultValue);
        SaveValue(defaultValue);
        updateValueAction(defaultValue);
    }
}
