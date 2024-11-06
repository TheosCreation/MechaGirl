using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private MovementController movementController;

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
    private Timer jumpTimer;
    private bool wasGrounded = true; // Track if the player was grounded in the previous frame

    [Header("Wall Jump")]
    [SerializeField] private float wallDistance = 1.0f;
    [SerializeField] private int maxWallJumps = 3;
    private int remainingWallJumps;
    private Vector3 wallNormal;

    [Header("Wall Running")]
    [SerializeField] private float maxWallRunSpeed = 2.0f;
    [SerializeField] private float wallRunDuration = 2.0f; // How long the wall run lasts
    public bool isWallRunning = false;
    private Timer wallRunTimer;

    [Header("Dash")]
    [SerializeField] public bool isDashing = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashForce = 15.0f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2.0f;
    Timer dashTimer;
    Timer dashCoolDownTimer;

    Vector2 movementInput = Vector2.zero;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        movementController = GetComponent<MovementController>();

        InputManager.Instance.playerInput.InGame.Jump.started += _ctx => Jump();

        jumpTimer = gameObject.AddComponent<Timer>();
        dashTimer = gameObject.AddComponent<Timer>();
        dashCoolDownTimer = gameObject.AddComponent<Timer>(); 
        wallRunTimer = gameObject.AddComponent<Timer>();

        remainingWallJumps = maxWallJumps; // Initialize remaining wall jumps
    }

    private void FixedUpdate()
    {
        CheckLanding();

        if (isDashing) return;

        movementInput = InputManager.Instance.MovementVector;
        //float targetRight = Mathf.InverseLerp(-1f, 1f, movementInput.x);
        //smoothRight = Mathf.SmoothDamp(smoothRight, targetRight, ref currentVelocity, walkingRightTransition);
        //playerController.weaponHolder.currentWeapon.UpdateWalkingAnimations(movementInput != Vector2.zero, smoothRight);
        if (movementInput == Vector2.zero)
        {
            movementController.movement = false;
            return;
        }

        if(isWallRunning)
        {
            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

            movementController.ResetVerticalVelocity();

            movementController.AddForce(wallForward * 5.0f);

            return;
        }


        

        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        movement = movement.normalized;

        movementController.MoveLocal(movement, maxWalkSpeed, acceleration, deceleration);
    }

    private void EndWallRun()
    {
        if (!isWallRunning) // Only start wall run if not already wall running
        {
            movementController.SetGravity(true);
            isWallRunning = true;

            // Reduce gravity while wall running
            //movementController. *= wallRunGravityScale;

            // Start the wall run timer
            wallRunTimer.StopTimer();
            wallRunTimer.SetTimer(wallRunDuration, EndWallRun);
        }
    }

    private void StartWallRun()
    {
        isWallRunning = false;
        movementController.SetGravity(false);
        // movementController. /= wallRunGravityScale;
    }

    void Jump()
    {
        if (movementController.isGrounded && canJump)
        {
            PerformJump();
        }
        else if (canJump && isNearWall() && remainingWallJumps > 0)
        {
            PerformWallJump();
        }
    }

    void PerformJump()
    {
        canJump = false;
        movementController.AddForce(Vector3.up * jumpForce);

        // Play a CameraJumpAnimation
        playerController.playerLook.PlayJumpAnimation(jumpDuration);

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
        }

        // Update the grounded state for the next frame
        wasGrounded = movementController.isGrounded;
    }
}
