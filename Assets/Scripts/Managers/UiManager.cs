using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private LevelCompleteMenuManager levelCompleteScreen;

    [SerializeField] private Transform weaponSwayTransform;
    [SerializeField] private Image weaponImage;
    public Image crosshairImage;
    [SerializeField] private FlashImage hitMarker;
    [SerializeField] private FlashImage hurtScreen;
    [SerializeField] private UiBar healthBar;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image weaponIconImage;

    [SerializeField] private GameObject tutorialHud;
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
    
        tutorialHud.SetActive(!(text == ""));
      
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
}
