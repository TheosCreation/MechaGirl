using UnityEngine;

public class WeaponAnim : MonoBehaviour
{
    private PlayerController player;

    [Header("Weapon Sway")]
    [SerializeField] private float positionalSway = 1f;
    [SerializeField] private float swaySmoothness = 1f;
    [SerializeField] private float maxSwayAmount = 0.2f; // Maximum sway limit

    [Header("Walking Bobbing")]
    [SerializeField] private float bobbingFrequency = 1.5f;
    [SerializeField] private float verticalBobbingAmplitude = 0.05f;
    [SerializeField] private float horizontalBobbingAmplitude = 0.05f;
    [SerializeField] private float bobbingHorizontalSmoothing = 5f;

    private Vector3 initialPosition = Vector3.zero;
    private Vector3 swayPositionOffset = Vector3.zero;
    private float bobbingTimer = 0f;
    private float targetBobbingDirection = 1f;
    private float currentBobbingDirection = 1f;
    private Vector3 curentPosition = Vector3.zero;

    private void Awake()
    {
        player = transform.root.GetComponent<PlayerController>();
    }

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        CalculateSway();
        Vector3 bobbingOffset = CalculateBobbing();

        // Calculate the combined final position and rotation including bobbing, strafe, and sway
        Vector3 finalPosition = initialPosition - (swayPositionOffset + bobbingOffset);

        // Apply the sway, strafe, and bobbing effects to the local position and rotation
        curentPosition = Vector3.Lerp(curentPosition, finalPosition, Time.deltaTime * swaySmoothness);
        UiManager.Instance.UpdateWeaponSwayPosition(curentPosition);
    }

    private void CalculateSway()
    {
        if (player.weaponHolder.currentWeapon == null) return;

        float mouseX = InputManager.Instance.currentMouseDelta.x * 0.1f;
        float mouseY = InputManager.Instance.currentMouseDelta.y * 0.1f;

        swayPositionOffset = new Vector3(mouseX, mouseY, 0) * positionalSway;

        // Clamp sway position to prevent excessive movement
        swayPositionOffset.x = Mathf.Clamp(swayPositionOffset.x, -maxSwayAmount, maxSwayAmount);
        swayPositionOffset.y = Mathf.Clamp(swayPositionOffset.y, -maxSwayAmount, maxSwayAmount);
    }
    private Vector3 CalculateBobbing()
    {
        if (player.weaponHolder.currentWeapon == null) return Vector3.zero;

        Vector3 movementVector = InputManager.Instance.MovementVector;

        if (movementVector.magnitude > 0.1f) // Check if the player is moving
        {
            // Increment the bobbing timer with a consistent rate
            bobbingTimer += Time.deltaTime * bobbingFrequency;

            currentBobbingDirection = Mathf.Lerp(currentBobbingDirection, targetBobbingDirection, Time.deltaTime * bobbingHorizontalSmoothing);

            // Use Mathf.Sin() for smooth, continuous up-and-down motion
            float verticalOffset = Mathf.Sin(bobbingTimer) * verticalBobbingAmplitude;

            float horizontalOffset = horizontalBobbingAmplitude * Mathf.Cos(bobbingTimer) * currentBobbingDirection;

            // Return vertical bobbing offset
            return new Vector3(horizontalOffset, verticalOffset, 0);
        }
        else
        {
            // Smoothly reduce bobbing to zero when the player stops moving
            float verticalOffset = Mathf.Sin(bobbingTimer) * verticalBobbingAmplitude;

            float horizontalOffset = horizontalBobbingAmplitude * Mathf.Cos(bobbingTimer) * currentBobbingDirection;
            
            return new Vector3(horizontalOffset, verticalOffset, 0);
        }
    }
}