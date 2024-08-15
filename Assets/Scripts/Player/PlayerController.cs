using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [HideInInspector] public Camera playerCamera;
    [HideInInspector] public PlayerLook playerLook;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public WeaponHolder weaponHolder;

    public int maxHealth = 100;
    private float health;
    public float Health
    {
        get => health;
        set
        {
            health = value;

            UiManager.Instance.UpdateHealthUi(health);

            if (health <= 0)
            {
                Die();
            }
        }
    }

    public event Action OnDeath;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        playerLook = GetComponent<PlayerLook>();
        playerMovement = GetComponent<PlayerMovement>();
        weaponHolder = GetComponentInChildren<WeaponHolder>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Health = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
        UiManager.Instance.FlashHurtScreen();
    }

    public void Heal(float healAmount)
    {
        float newHealth = Health + healAmount;
        Health = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    private void Die()
    {
        //handle death by destroying the gameobject then cleaning up the scene, show death screen with restart option
        //Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("wow ive hit a weapon lets try pick it up");
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            weapon.PickUp(weaponHolder);
        }
    }
}
