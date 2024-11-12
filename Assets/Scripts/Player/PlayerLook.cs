using Runtime;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Tab("Settings")]
    [Header("Looking")]
    public float lookSensitivity = 1f;

    [Header("Camera Tilt")]
    [HideInInspector] public int tiltStatus = 1;
    public float tiltAmount = 2.5f;
    [SerializeField] private float tiltSmoothTime = 0.1f;

    [Header("Jump Animation")]
    [SerializeField] private float jumpAnimTiltAngle = 5f; // Adjust this value for more or less tilt
    [SerializeField] private bool playingJumpAnim = false;
    private float jumpAnimDuration = 0.0f; // Duration of the jump effect
    private float jumpElapsedTime = 0f;

    [Header("Land Animation")]
    [SerializeField] private float landAnimDuration = 0.3f; // Duration of the land effect
    [SerializeField] private float landAnimTiltAngle = -3f; // Tilt angle for landing
    [SerializeField] private bool playingLandAnim = false;
    private float landElapsedTime = 0f;

    [Tab("Setup")]
    [Header("Body Transforms")]
    [SerializeField] private Transform cameraTransform;
    [HideInInspector] public float currentXRotation = 0f;

    private float currentTilt = 0;
    private float tiltVelocity = 0;
    private Rigidbody rb;

    [Header("Screen Shake")]
    [HideInInspector] public float shakeAmount = 1.0f;
    private float shakeMagnitude = 0.1f;

    private float shakeTimeRemaining = 0f;
    private Vector3 originalCameraPosition;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip landingClip;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalCameraPosition = cameraTransform.localPosition;
    }

    private void Update()
    {
        Look();
        HandleScreenShake();
        HandleJumpAnimation();
        HandleLandAnimation();
    }

    private void Look()
    {
        Vector2 currentMouseDelta = InputManager.Instance.currentMouseDelta;

        // Calculate vertical rotation and clamp it
        currentXRotation -= currentMouseDelta.y * lookSensitivity;
        currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);

        // Calculate horizontal rotation as a Quaternion
        Quaternion horizontalRotation = Quaternion.Euler(0f, currentMouseDelta.x * lookSensitivity, 0f);

        // Apply the horizontal rotation to the player body
        rb.MoveRotation(rb.rotation * horizontalRotation);

        if (tiltStatus == 1)
        {
            float targetTilt = InputManager.Instance.MovementVector.x * tiltAmount;

            // Smoothly interpolate the current tilt towards the target value
            currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmoothTime);
        }

        // Rotate the camera vertically and apply tilt
        cameraTransform.localRotation = Quaternion.Euler(currentXRotation, 0f, -currentTilt);
    }

    public void TriggerScreenShake(float duration, float magnitude)
    {
        shakeTimeRemaining = duration;
        shakeMagnitude = magnitude * shakeAmount;
    }

    private void HandleScreenShake()
    {
        if (shakeTimeRemaining > 0)
        {
            cameraTransform.localPosition = originalCameraPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            cameraTransform.localPosition = originalCameraPosition;
        }
    }

    public void PlayJumpAnimation(float jumpDuration)
    {
        jumpAnimDuration = jumpDuration;
        if (!playingJumpAnim)
        {
            playingJumpAnim = true;
            jumpElapsedTime = 0f;
        }
    }

    private void HandleJumpAnimation()
    {
        if (playingJumpAnim)
        {
            float halfDuration = jumpAnimDuration / 2f;
            Quaternion originalRotation = cameraTransform.localRotation;

            if (jumpElapsedTime < halfDuration)
            {
                // Rotate forwards (downwards tilt)
                float tilt = Mathf.Lerp(0f, jumpAnimTiltAngle, jumpElapsedTime / halfDuration);
                cameraTransform.localRotation = originalRotation * Quaternion.Euler(-tilt, 0f, 0f);
            }
            else if (jumpElapsedTime < jumpAnimDuration)
            {
                // Rotate back to original
                float tilt = Mathf.Lerp(jumpAnimTiltAngle, 0f, (jumpElapsedTime - halfDuration) / halfDuration);
                cameraTransform.localRotation = originalRotation * Quaternion.Euler(-tilt, 0f, 0f);
            }
            else
            {
                // Ensure the camera returns to its original rotation
                cameraTransform.localRotation = originalRotation;
                playingJumpAnim = false;
            }

            jumpElapsedTime += Time.deltaTime;
        }
    }

    public void PlayLandAnimation()
    {
        if (!playingLandAnim)
        {
            audioSource.PlayOneShot(landingClip);
            playingLandAnim = true;
            landElapsedTime = 0f;
        }
    }

    private void HandleLandAnimation()
    {
        if (playingLandAnim)
        {
            float halfDuration = landAnimDuration / 2f;
            Quaternion originalRotation = cameraTransform.localRotation;

            if (landElapsedTime < halfDuration)
            {
                // Rotate backwards (upwards tilt)
                float tilt = Mathf.Lerp(0f, landAnimTiltAngle, landElapsedTime / halfDuration);
                cameraTransform.localRotation = originalRotation * Quaternion.Euler(-tilt, 0f, 0f);
            }
            else if (landElapsedTime < landAnimDuration)
            {
                // Rotate back to original
                float tilt = Mathf.Lerp(landAnimTiltAngle, 0f, (landElapsedTime - halfDuration) / halfDuration);
                cameraTransform.localRotation = originalRotation * Quaternion.Euler(-tilt, 0f, 0f);
            }
            else
            {
                // Ensure the camera returns to its original rotation
                cameraTransform.localRotation = originalRotation;
                playingLandAnim = false;
            }

            landElapsedTime += Time.deltaTime;
        }
    }
}
