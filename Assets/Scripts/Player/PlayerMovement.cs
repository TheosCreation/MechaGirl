using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private MovementController movementController;
    private CapsuleCollider capsuleCollider;

    [Header("Movement")]
    [SerializeField] private float maxWalkSpeed = 2.0f;
    [SerializeField] private float acceleration = 5.0f;
    [SerializeField] private float deceleration = 2.0f;

    [Header("Animations")]
    [SerializeField] private float walkingRightTransition = 20.0f;
    float smoothRight;
    private float currentVelocity;

    [Header("Jump")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float jumpDuration = 0.1f;
    [SerializeField] private AudioClip jumpingClip;
    private Timer jumpTimer;
    private bool wasGrounded = true; // Track if the player was grounded in the previous frame

    [Header("Wall Jump")]
    [SerializeField] private float wallDistance = 1.0f;
    [SerializeField] private int maxWallJumps = 3;
    private int remainingWallJumps;
    private Vector3 wallNormal;

    [Header("Dash")]
    public bool isDashing = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashForce = 15.0f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2.0f;
    Timer dashTimer;
    Timer dashCoolDownTimer;

    [Header("Sliding")]
    public bool isSliding = false;
    [SerializeField] private float slideSpeed = 20.0f;
    [SerializeField] private float slideAcceleration = 10.0f;
    [SerializeField, Range(0f, 1f)] private float slideHeightMultiplier = 0.5f;
    [SerializeField, Range(0f, 1f)] private float slideHorizontalMovement = 0.5f;
    [SerializeField] private float slideTiltAmount = 5f;
    [SerializeField] private float slideCancelDelay = 0.1f;
    private Vector3 initialSlideDirection;
    private bool canCancelSlide = false;
    private Timer slideCancelTimer;
    private float originalCapsuleHeight = 0f;
    private float originalCapsuleCenterY = 0f;

    [Header("Gravity Reduction")]
    [SerializeField] private float reducedGravityFactor = 0.1f;
    private bool isReducingGravity = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSource2;
    Vector2 movementInput = Vector2.zero;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        movementController = GetComponent<MovementController>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        InputManager.Instance.playerInput.InGame.Jump.started += _ctx => Jump();
        InputManager.Instance.playerInput.InGame.Jump.canceled += ctx => StopReducingGravity();
        InputManager.Instance.playerInput.InGame.Slide.started += ctx => StartSliding();
        InputManager.Instance.playerInput.InGame.Slide.canceled += ctx => EndSlide();

        jumpTimer = gameObject.AddComponent<Timer>();
        dashTimer = gameObject.AddComponent<Timer>();
        dashCoolDownTimer = gameObject.AddComponent<Timer>(); 
        slideCancelTimer = gameObject.AddComponent<Timer>();

        remainingWallJumps = maxWallJumps; // Initialize remaining wall jumps
        originalCapsuleHeight = capsuleCollider.height;
        originalCapsuleCenterY = capsuleCollider.center.y;
    }

    private void FixedUpdate()
    {
        if (InputManager.Instance.playerInput.InGame.Jump.ReadValue<float>() > 0f && !movementController.isGrounded)
        {
            if (movementController.GetVerticalVelocity() < 0f)
            {
                StartReducingGravity();
            }
        }
        else if (isReducingGravity)
        {
            StopReducingGravity();
        }



        CheckLanding();

        if (isDashing)
        {
            audioSource.Stop();
            return;
        }
        if(!movementController.isGrounded)
        {
            audioSource.Stop();
        }

        Movement();
       
    }

    private void Movement()
    {
        movementInput = InputManager.Instance.MovementVector;
        //float targetRight = Mathf.InverseLerp(-1f, 1f, movementInput.x);
        //smoothRight = Mathf.SmoothDamp(smoothRight, targetRight, ref currentVelocity, walkingRightTransition);
        //playerController.weaponHolder.currentWeapon.UpdateWalkingAnimations(movementInput != Vector2.zero, smoothRight);
        if (movementInput == Vector2.zero)
        {
            movementController.movement = false;
            audioSource.Stop();
            return;
        }

        if (!audioSource.isPlaying && movementController.isGrounded)
        {
            audioSource.Play();
        }


        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        movement = movement.normalized;

        movementController.MoveLocal(movement, maxWalkSpeed, acceleration, deceleration);
        if (isSliding)
        {
            if (!movementController.isGrounded) return;

            EndDash();

            // Calculate the right direction using cross product
            Vector3 slideDirectionRight = Vector3.Cross(Vector3.up, initialSlideDirection);

            Vector3 blendedDirection = initialSlideDirection + (movement.x * slideDirectionRight * slideHorizontalMovement);
            blendedDirection.Normalize();

            movementController.MoveWorld(blendedDirection, slideSpeed, slideAcceleration, deceleration);

            if (movementController.GetLinearVelocityMagnitude() < (maxWalkSpeed / 2) && canCancelSlide)
            {
                EndSlide();
            }
            return;
        }
    }

    void Jump()
    {
        if (movementController.isGrounded && canJump)
        {
            PerformJump();
        }
      
      /*  else if (canJump && isNearWall() && remainingWallJumps > 0)
        {
            PerformWallJump();
        }*/
    }

    void PerformJump()
    {
        if(isSliding) EndSlide();

        canJump = false;
        movementController.AddForce(Vector3.up * jumpForce);

        // Play a CameraJumpAnimation
        playerController.playerLook.PlayJumpAnimation(jumpDuration);

        audioSource2.PlayOneShot(jumpingClip);

        isJumping = true;
        jumpTimer.StopTimer();
        jumpTimer.SetTimer(jumpDuration, JumpEnd);
    }

    void PerformWallJump()
    {
        canJump = false;

        Vector3 pushDirection = wallNormal.normalized;

        movementController.ResetVerticalVelocity();
        // Apply the force in the calculated jump direction
        movementController.AddForce((Vector3.up + pushDirection) * jumpForce);

        // Play a CameraJumpAnimation
        playerController.playerLook.PlayJumpAnimation(jumpDuration);

        isJumping = true;
        remainingWallJumps--; // Decrease remaining wall jumps
        jumpTimer.StopTimer();
        jumpTimer.SetTimer(jumpDuration, JumpEnd);
    }

    void JumpEnd()
    {
        isJumping = false;
        canJump = true;
    }
    private void StartReducingGravity()
    {
        if (!isReducingGravity )
        {
            isReducingGravity = true;
            movementController.ReduceGravity(reducedGravityFactor);
        }
    }

    private void StopReducingGravity()
    {
        if (isReducingGravity)
        {
            isReducingGravity = false;
            movementController.RestoreGravity();
        }
    }

    private bool isNearWall()
    {
        // Define the directions to shoot the raycasts
        Vector3[] directions = new Vector3[]
        {
        transform.forward, // Forward
        -transform.forward, // Backward
        transform.right, // Right
        -transform.right // Left
        };

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, wallDistance))
            {
                Debug.DrawRay(transform.position, direction * wallDistance, Color.red);
                wallNormal = hit.normal;
                return true;
            }
        }

        return false;
    }

    public void Teleport(Vector3 targetPosition)
    {
        Vector3 dashDirection = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        float scaledDashForce = dashForce * (1 + (distance * 0.1f)) * 0.5f;
        Dash(dashDirection, scaledDashForce, dashDuration * 1.5f, true);
    }

    public void Dash(Vector3 dashDirection, float dashForce, float dashDuration, bool ignoreInput = false)
    {
        if (canDash)
        {
            if (isSliding) EndSlide();

            // If input detected then apply it
            if (movementInput.sqrMagnitude > Mathf.Epsilon && !ignoreInput)
            {
                dashDirection = new Vector3(movementInput.x, 0, movementInput.y);
                dashDirection.Normalize();
                dashDirection = transform.TransformDirection(dashDirection);
            }

            movementController.ResetVerticalVelocity();
            movementController.ResetHorizontalVelocity();
            movementController.AddForce(dashDirection * dashForce);
            movementController.SetFriction(false);
            isDashing = true;
            canDash = false;
            dashTimer.StopTimer();
            dashTimer.SetTimer(dashDuration, EndDash);

            dashCoolDownTimer.StopTimer();
            dashCoolDownTimer.SetTimer(dashCooldown, RefreshDash);
        }
    }

    void EndDash()
    {
        isDashing = false;
        movementController.SetFriction(true);
    }

    void RefreshDash()
    {
        movementController.SetFriction(true);
        canDash = true;
    }

    private void CheckLanding()
    {
        // Check if the player just landed
        if (!wasGrounded && movementController.isGrounded && !isJumping)
        {
            playerController.playerLook.PlayLandAnimation();
            remainingWallJumps = maxWallJumps;
            StopReducingGravity();
        }

        // Update the grounded state for the next frame
        wasGrounded = movementController.isGrounded;
    }
    private void StartSliding()
    {
        if (movementController.isGrounded)
        {
            // Calculate the new height based on the multiplier
            float newHeight = originalCapsuleHeight * slideHeightMultiplier;

            // Adjust the capsule collider's height
            capsuleCollider.height = newHeight;

            // Adjust the collider center so the player doesn't move up or down unexpectedly
            capsuleCollider.center = new Vector3(capsuleCollider.center.x,
                                                 originalCapsuleCenterY + (newHeight / 2), // Adjust based on the original center Y
                                                 capsuleCollider.center.z);

            // Ensure the player's position stays consistent by adjusting the player height (if necessary)
            // Example: Adjusting the Y position of the player's transform to keep them grounded
            Vector3 playerPosition = transform.position;
            playerPosition.y += (newHeight - originalCapsuleHeight) / 2; // Maintain the player's position relative to the ground
            transform.position = playerPosition;

            // Set sliding state and disable the ability to cancel sliding until the timer runs out
            isSliding = true;
            canCancelSlide = false;
            slideCancelTimer.SetTimer(slideCancelDelay, () => { canCancelSlide = true; });

            // Set the camera tilt effect while sliding
            playerController.playerLook.tiltAmount = slideTiltAmount;

            // Get the player's movement input and determine the slide direction
            Vector2 inputVector = InputManager.Instance.playerInput.InGame.Movement.ReadValue<Vector2>();

            // Default to forward direction for sliding
            initialSlideDirection = transform.forward;

            // If input is detected, adjust the slide direction accordingly
            if (inputVector.sqrMagnitude > 0)
            {
                initialSlideDirection = new Vector3(inputVector.x, 0, inputVector.y);
                initialSlideDirection.Normalize();
                initialSlideDirection = transform.TransformDirection(initialSlideDirection);
            }

            // Optional: Play sliding sound and particle effects
            // slidingAudioSource.clip = slidingSoundEffect;
            // slidingAudioSource.Play();
            // slidingParticles.Play();
        }
    }

    private void EndSlide()
    {
        if (isSliding)
        {
            // Restore the original height of the capsule collider
            capsuleCollider.height = originalCapsuleHeight;

            // Adjust the center of the capsule collider to match the original center
            capsuleCollider.center = new Vector3(capsuleCollider.center.x,
                                                 originalCapsuleCenterY + (originalCapsuleHeight / 2),
                                                 capsuleCollider.center.z);

            // Ensure the player's position stays consistent when the slide ends
            Vector3 playerPosition = transform.position;
            playerPosition.y += (originalCapsuleHeight - capsuleCollider.height) / 2; // Adjust position to match original height
            transform.position = playerPosition;

            // Set the sliding state to false
            isSliding = false;

            // Restore the original camera tilt amount
            playerController.playerLook.tiltAmount = playerController.playerLook.originalTiltAmount;

            // Optional: Play ending slide sound and stop particles
            // slidingAudioSource.Stop();
            // audioSource.PlayOneShot(slideEndSoundEffect);
            // slidingParticles.Stop();
        }
    }


}
