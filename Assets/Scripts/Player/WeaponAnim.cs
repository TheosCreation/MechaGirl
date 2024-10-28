using UnityEngine;

public class WeaponAnim : MonoBehaviour
{
    private PlayerController player;
    [Header("Weapon Sway")]
    [SerializeField] private float positionalSway = 1f;
    [SerializeField] private float swaySmoothness = 1f;

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
    }

    private Vector3 CalculateBobbing()
    {
        if (player.weaponHolder.currentWeapon == null) return Vector3.zero;

        Vector3 movementVector = InputManager.Instance.MovementVector;

        if (movementVector.magnitude > 0.1f) // Check if the player is moving
        {
            // Set target bobbing direction based on initial x input
            if (movementVector.x < 0)
            {
                targetBobbingDirection = -1f; // Moving left
            }
            else if (movementVector.x > 0)
            {
                targetBobbingDirection = 1f; // Moving right
            }

            // Smoothly interpolate towards the target direction
            currentBobbingDirection = Mathf.Lerp(currentBobbingDirection, targetBobbingDirection, Time.deltaTime * bobbingHorizontalSmoothing);

            // Increment bobbing timer
            bobbingTimer += Time.deltaTime * bobbingFrequency;

            // Calculate vertical bobbing (up/down)
            float verticalOffset = -Mathf.Abs(verticalBobbingAmplitude * Mathf.Sin(bobbingTimer)) ;

            // Calculate horizontal bobbing (left/right) with lerped direction
            float horizontalOffset = horizontalBobbingAmplitude * Mathf.Cos(bobbingTimer) * currentBobbingDirection;

            // Apply bobbing reduction for aiming and return the final bobbing vector
            return new Vector3(horizontalOffset, verticalOffset, 0);
        }
        else
        {
            // Reset the timer and bobbing direction when not moving
            bobbingTimer = 0f;
            return Vector3.zero;
        }
    }
}