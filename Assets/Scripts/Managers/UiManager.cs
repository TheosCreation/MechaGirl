using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [Header("UI Screens")]
    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private LevelCompleteMenuManager levelCompleteScreen;

    [Header("Player UI Elements")]
    [SerializeField] private Transform weaponSwayTransform;
    [SerializeField] private Image weaponImage;
    [SerializeField] private FlashImage hitMarker;
    [SerializeField] private FlashImage hurtScreen;
    [SerializeField] private UiBar healthBar;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image weaponIconImage;

    [Header("Game Progress UI")]
    [SerializeField] private UiBar bossBar;
    [SerializeField] private Transform keycardHolder;

    [Header("New Weapon Animation")]
    [SerializeField] private Image newWeapon;
    public float animationDuration = 0.5f;
    private Vector3 startPosition;
    private Color startColor;
    private HashSet<Type> pickedUpWeaponTypes = new HashSet<Type>();

    [Header("Tutorial")]
    [SerializeField] private TMP_Text tutorialText;

    //public Image image;
    //public UiBar bar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        InputManager.Instance.EnableInGameInput();
        deathScreen.SetActive(false);
        levelCompleteScreen.gameObject.SetActive(false);
        bossBar.gameObject.SetActive(false);
        SetTutorialText("");

        if (newWeapon != null)
        {
            startPosition = newWeapon.transform.position;
            startColor = newWeapon.color;
            newWeapon.color = new Color(startColor.r, startColor.g, startColor.b, 0);
            newWeapon.transform.position -= Vector3.up * 50f; // Adjust this value as needed
        }
    }

    public void PauseMenu(bool isPaused)
    {
        pauseMenu.SetActive(isPaused);
        playerHud.SetActive(!isPaused);
    }

    public void OpenPlayerHud()
    {
        playerHud.SetActive(true);
        deathScreen.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void OpenDeathScreen()
    {
        deathScreen.SetActive(true);
        playerHud.SetActive(false);
    }

    public void UpdateHealthUi(float health)
    {
        healthBar.UpdateBar(health);
    }
    public void UpdateAmmoUi(int ammo)
    {
        ammoText.text = ammo > 0 ? ammo.ToString() : "";  // Set to empty string if ammo is 0
    }
    public void SetTutorialText(string text)
    {
        tutorialText.text = text;

        tutorialText.transform.parent.gameObject.SetActive(!(text == ""));
      
    }
    public void UpdateWeaponIcon(Sprite weaponIcon)
    {
        weaponIconImage.sprite = weaponIcon;
    }

    public void UpdateWeaponImage(Sprite sprite)
    {
        weaponImage.sprite = sprite;
    }

    public void UpdateWeaponAnimationPosition(Vector3 position)
    {
        // Get the screen's center point in world space
        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane));

        // Adjust the position so that (0,0) is the screen center
        Vector3 adjustedPosition = screenCenter + position;

        // Update the weapon's local position relative to its parent
        weaponImage.transform.localPosition = adjustedPosition;
    }


    public void UpdateWeaponAnimationRotation(Vector3 rotation)
    {
        weaponImage.transform.localRotation = Quaternion.Euler(rotation);
    }

    public void UpdateWeaponSwayPosition(Vector3 position)
    {
        // Get the screen's center point
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Adjust the position so that (0,0) is the screen center
        Vector3 adjustedPosition = screenCenter + position;

        weaponSwayTransform.transform.position = adjustedPosition;
    }

    public void UpdateWeaponSwayRotation(Quaternion rotation)
    {
        weaponSwayTransform.rotation = rotation;
    }

    public void FlashHitMarker()
    {
        hitMarker.Play();
    }

    public void FlashHurtScreen()
    {
        hurtScreen.Play(); 
    }
    public void UpdateWeaponImageColor(Color newColor)
    {
        weaponImage.color = newColor;
    }

    public void OpenLevelCompleteScreen(float levelCompleteTime, int currentLevelNumber)
    {
        playerHud.SetActive(false); 
        pauseMenu.SetActive(false);
        levelCompleteScreen.gameObject.SetActive(true);
        levelCompleteScreen.UpdateLevelNumber(currentLevelNumber);
        levelCompleteScreen.UpdateTimeText(levelCompleteTime);
        levelCompleteScreen.UpdateBestTimeText(GameManager.Instance.GameState.GetBestTimeForCurrentLevel());
    }

    public void SetBossBarStatus(bool status, string bossName = "BOSS NAME")
    {
        bossBar.gameObject.SetActive(status);
        bossBar.text.text = bossName;
    }

    public void SetBossBarHealth(float percentage)
    {
        bossBar.UpdateBar(percentage * 100.0f);
    }

    public void AddKeyCardIcon(Keycard keycard)
    {
        GameObject icon = Instantiate(keycard.prefabIcon, keycardHolder);
        icon.GetComponent<Image>().color = keycard.colorTag;
    }

    public void PickUp(Sprite iconSprite)
    {
        newWeapon.gameObject.SetActive(true);
        if (newWeapon != null && iconSprite != null)
        {
            newWeapon.sprite = iconSprite;
            StartCoroutine(FadeAndMove());
        }
    }

    IEnumerator FadeAndMove()
    {
        float elapsedTime = 0f;
        Vector3 endPosition = startPosition;
        Color endColor = startColor;

        while (elapsedTime < animationDuration)
        {
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            t = t * t * (3f - 2f * t); 

            newWeapon.color = Color.Lerp(new Color(endColor.r, endColor.g, endColor.b, 0), endColor, t);
            newWeapon.transform.position = Vector3.Lerp(startPosition - Vector3.up * 50f, endPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        newWeapon.color = endColor;
        newWeapon.transform.position = endPosition;
        newWeapon.gameObject.SetActive(false);
    }
}
