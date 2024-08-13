using Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private MovementController movementController;

    [Tab("Settings")]
    [Header("Movement")]
    public bool isMoving = false;
    [SerializeField] private float maxSpeed = 2.0f;
    [SerializeField] private float acceleration = 5.0f;
    [SerializeField] private float deceleration = 2.0f;
    [Header("Jump")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float jumpDuration = 0.1f;
    private Timer jumpTimer;
    private bool wasGrounded = true; // Track if the player was grounded in the previous frame

    [Header("Dash")]
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashForce = 15.0f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;

    //[Tab("Setup")]
    Vector2 movementInput = Vector2.zero;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        movementController = GetComponent<MovementController>();

        InputManager.Instance.playerInput.InGame.Jump.started += _ctx => Jump();
        InputManager.Instance.playerInput.InGame.Dash.started += _ctx => Dash();

        jumpTimer = gameObject.AddComponent<Timer>();
    }

    void Jump()
    {
        if (movementController.isGrounded && canJump)
        {
            canJump = false;
            movementController.AddForce(Vector3.up * jumpForce);

            // Play a CameraJumpAnimation
            playerController.playerLook.PlayJumpAnimation(jumpDuration);

            isJumping = true;
            jumpTimer.SetTimer(jumpDuration, JumpEnd);
        }
    }

    void JumpEnd()
    {
        isJumping = false;
        canJump = true;
    }

    void Dash()
    {
        if (canDash)
        {
            Vector3 dashDirection = transform.forward;
            // If input detected then apply it
            if (movementInput.sqrMagnitude > Mathf.Epsilon)
            {
                dashDirection = new Vector3(movementInput.x, 0, movementInput.y);

                dashDirection.Normalize();

                dashDirection = transform.TransformDirection(dashDirection);
            }

            movementController.ResetVerticalVelocity();
            movementController.ResetHorizontalVelocity();
            movementController.AddForce(dashDirection * dashForce);
            movementController.SetFriction(false);
            canDash = false;
            isDashing = true;
            // Allow dashing after cooldown
            Timer dashTimer = gameObject.AddComponent<Timer>();
            dashTimer.SetTimer(dashDuration, EndDash);
            Destroy(dashTimer, dashDuration + (dashDuration / 10));

            // Allow dashing after cooldown
            Timer refreshTimer = gameObject.AddComponent<Timer>();
            refreshTimer.SetTimer(dashCooldown, RefreshDash);
            Destroy(refreshTimer, dashCooldown + (dashCooldown / 10));
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

    private void FixedUpdate()
    {
        CheckLanding();

        if (isDashing) return;

        movementInput = InputManager.Instance.MovementVector;
        if (movementInput == Vector2.zero)
        {
            isMoving = false;
            movementController.movement = isMoving;
            playerController.weaponHolder.currentWeapon.UpdateWalking(isMoving);
            return;
        }

        isMoving = true;
        playerController.weaponHolder.currentWeapon.UpdateWalking(isMoving);

        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        movement = movement.normalized;

        movementController.MoveLocal(movement, maxSpeed, acceleration, deceleration);
    }

    private void CheckLanding()
    {
        // Check if the player just landed
        if (!wasGrounded && movementController.isGrounded && !isJumping)
        {
            // Play the land animation
            playerController.playerLook.PlayLandAnimation();
        }

        // Update the grounded state for the next frame
        wasGrounded = movementController.isGrounded;
    }
}
