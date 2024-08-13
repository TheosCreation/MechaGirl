using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathScreen;

    [SerializeField] private Image weaponImage;
    [SerializeField] private FlashImage hitMarker;
    [SerializeField] private FlashImage hurtScreen;
    [SerializeField] private UiBar healthBar;

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
    }

    public void PauseMenu(bool isPaused)
    {
        pauseMenu.SetActive(isPaused);
        playerHud.SetActive(!isPaused);
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

    public void UpdateWeaponImage(Sprite sprite)
    {
        weaponImage.sprite = sprite;
    }

    public void FlashHitMarker()
    {
        hitMarker.Play();
    }

    public void FlashHurtScreen()
    {
        hurtScreen.Play(); 
    }
}
