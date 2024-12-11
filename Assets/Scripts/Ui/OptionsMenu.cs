using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem.Samples.RebindUI;

public class OptionsMenu : UiPage
{
    [SerializeField] private RebindActionUI[] rebindingButtons;
    [SerializeField] private GameObject generalPage;
    [SerializeField] private GameObject controlsPage;
    [SerializeField] private GameObject graphicsPage;
    [SerializeField] private GameObject audioPage;

    [Header("Master Volume Slider Option")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private Button masterVolumeResetButton;

    [Header("SFX Volume Slider Option")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeText;
    [SerializeField] private Button sfxVolumeResetButton;

    [Header("Music Volume Slider Option")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private Button musicVolumeResetButton;

    [Header("Sensitivity Slider Option")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityText;
    [SerializeField] private Button sensitivityResetButton;

    [Header("Look Smoothing Option")]
    [SerializeField] private Slider lookSmoothingSlider;
    [SerializeField] private TMP_Text lookSmoothingText;
    [SerializeField] private Button lookSmoothingResetButton;

    [Header("Fov Option")]
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TMP_Text fovText;
    [SerializeField] private Button fovResetButton;

    [Header("Tilt Option")]
    [SerializeField] private Toggle tiltToggle;
    [SerializeField] private Button tiltResetButton;

    [Header("Fullscreen Option")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button fullscreenButton;

    [Header("vSync Option")]
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Button vSyncButton;

    [Header("Screen Shake Option")]
    [SerializeField] private Slider screenShakeSlider;
    [SerializeField] private TMP_Text screenShakeText;
    [SerializeField] private Button screenShakeResetButton;

    [Header("Graphics Quality Option")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button qualityResetButton;

    [Header("Resolution Option")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Button resolutionResetButton;

    [Header("Speedrun Option")]
    [SerializeField] private Toggle speedrunToggle;
    [SerializeField] private Button speedrunResetButton;


    private OptionsBase[] options;

    private void Awake()
    {
        options = new OptionsBase[]
        {
            CreateSliderOption(SettingsManager.Instance.options.masterVolume, masterVolumeSlider, masterVolumeText, masterVolumeResetButton, SettingsManager.Instance.UpdateVolume, true),
            CreateSliderOption(SettingsManager.Instance.options.sfxVolume, sfxVolumeSlider, sfxVolumeText, sfxVolumeResetButton, SettingsManager.Instance.UpdateVolume, true),
            CreateSliderOption(SettingsManager.Instance.options.musicVolume, musicVolumeSlider, musicVolumeText, musicVolumeResetButton, SettingsManager.Instance.UpdateVolume, true),
            CreateSliderOption(SettingsManager.Instance.options.sensitivity, sensitivitySlider, sensitivityText, sensitivityResetButton, SettingsManager.Instance.UpdateSensitivity, true),
            CreateSliderOption(SettingsManager.Instance.options.lookSmoothing, lookSmoothingSlider, lookSmoothingText, lookSmoothingResetButton, SettingsManager.Instance.UpdateLookSmoothing, true),
            CreateSliderOption(SettingsManager.Instance.options.screenShake, screenShakeSlider, screenShakeText, screenShakeResetButton, SettingsManager.Instance.UpdateScreenShake, true, "SHAKE IT OFF!!!"),
            CreateSliderOption(SettingsManager.Instance.options.fov, fovSlider, fovText, fovResetButton, SettingsManager.Instance.UpdateFov, false, "FISH EYE LENS!!!"),

            CreateToggleOption(SettingsManager.Instance.options.tilt, tiltToggle, tiltResetButton, SettingsManager.Instance.UpdateTilt),
            CreateToggleOption(SettingsManager.Instance.options.fullscreen, fullscreenToggle, fullscreenButton, SettingsManager.Instance.UpdateFullscreen),
            CreateToggleOption(SettingsManager.Instance.options.vSync, vSyncToggle, vSyncButton, SettingsManager.Instance.UpdateVSync),
            CreateToggleOption(SettingsManager.Instance.options.speedrun, speedrunToggle, speedrunResetButton, SettingsManager.Instance.UpdateSpeedrunMode),

            CreateDropdownOption(SettingsManager.Instance.options.graphicsQuality, qualityDropdown, qualityResetButton, SettingsManager.Instance.UpdateGraphicsQuality),
            CreateDropdownOption(SettingsManager.Instance.options.resolution, resolutionDropdown, resolutionResetButton, SettingsManager.Instance.UpdateScreenResolution)
        };
    }

    private OptionsSlider CreateSliderOption(FloatSetting setting, Slider slider, TMP_Text text, Button resetButton, System.Action<FloatSetting> updateAction, bool isPercentage = false, string maxValueText = "")
    {
        return new OptionsSlider(slider, text, resetButton, value => updateAction(setting), setting, isPercentage, maxValueText);
    }

    private OptionsToggle CreateToggleOption(BoolSetting setting, Toggle toggle, Button resetButton, System.Action<BoolSetting> updateAction)
    {
        return new OptionsToggle(toggle, resetButton, value => updateAction(setting), setting);
    }

    private OptionsDropdown CreateDropdownOption(IntSetting setting, TMP_Dropdown dropdown, Button resetButton, System.Action<IntSetting> updateAction)
    {
        return new OptionsDropdown(dropdown, resetButton, value => updateAction(setting), setting);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var option in options)
        {
            option.Initialize();
        }
        if (DiscordManager.Instance)
        {
            DiscordManager.Instance.ChangeActivity(Gamemode.Menu, 0, "Options Menu");
        }
    }

    public void UpdateOptions()
    {
        foreach (var option in options)
        {
            option.Update();
        }
    }
    
    public void ResetToDefaultsOptions()
    {
        foreach (var option in options)
        {
            option.ResetToDefault();
        }

        foreach(var rebindOption in rebindingButtons)
        {
            rebindOption.ResetToDefault();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (var option in options)
        {
            option.CleanUp();
        }
    }

    public void Back()
    {
        IMenuManager menuOwner = GetComponentInParent<IMenuManager>();
        if (menuOwner != null)
        {
            menuOwner.Back();
        }
        else
        {
            Debug.LogWarning("No IMenuManager found in parent hierarchy!");
        }
    }

    public void OpenGeneralPage()
    {
        generalPage.SetActive(true);
        graphicsPage.SetActive(false);
        controlsPage.SetActive(false);
        audioPage.SetActive(false);
    }

    public void OpenControlsPage()
    {
        controlsPage.SetActive(true);
        generalPage.SetActive(false);
        graphicsPage.SetActive(false);
        audioPage.SetActive(false);
    }

    public void OpenGraphicsPage()
    {
        graphicsPage.SetActive(true);
        controlsPage.SetActive(false);
        generalPage.SetActive(false);
        audioPage.SetActive(false);
    }

    public void OpenAudioPage()
    {
        audioPage.SetActive(true);
        generalPage.SetActive(false);
        controlsPage.SetActive(false);
        graphicsPage.SetActive(false);
    }

    public void UpdateRebinds()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.LoadBindingOverrides();
        }
    }

    public void ResetAllSettings()
    {
        SettingsManager.Instance.ResetAllSettings();
        ResetToDefaultsOptions();
        
    }
}