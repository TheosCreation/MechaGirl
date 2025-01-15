using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [Header("Movement specifics")]
    [SerializeField] LayerMask groundMask;
    public float movementSpeed = 14f;
    [Range(0f, 1f)]
    public float crouchSpeedMultiplier = 0.248f;
    [Range(0.01f, 0.99f)]
    public float movementThrashold = 0.01f;
    [Space(2)]

    public float acceleration = 0.2f;
    public float airAcceleration = 0.2f;
    public float decceleration = 0.1f;
    public float airDecceleration = 0.1f;
    public float slowDownDecceleration = 0.1f; // for when we are moving faster than walk speed

    [Header("Jump and gravity specifics")]
    [SerializeField] private AudioClip jumpingClip;
    public float jumpVelocity = 20f;
    public float jumpTime = 0.3f;
    public float fallMultiplier = 1.7f;
    public float holdJumpMultiplier = 5f;
    [Range(0f, 1f)]
    public float frictionAgainstFloor = 0.3f;
    [Range(0f, 0.99f)]
    public float frictionAgainstWall = 0.839f;
    [Space(2)]

    [Header("Wall slide specifics")]
    public float wallCheckerThrashold = 0.8f;
    public float hightWallCheckerChecker = 0.5f;
    [Space(2)]
    public float jumpFromWallMultiplier = 30f;
    public float multiplierVerticalLeap = 1f;
    [SerializeField] private int maxWallJumps = 3;
    private int remainingWallJumps = 0;

    [Header("Dash")]
    public bool isDashing = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashForce = 15.0f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2.0f;
    Timer dashTimer;
    Timer dashCoolDownTimer;

    [Header("Sliding")]
    [SerializeField] private float slideSpeed = 20.0f;
    [SerializeField] private float slideAcceleration = 10.0f;
    [SerializeField, Range(0f, 1f)] private float slideHorizontalMovement = 0.5f;
    [SerializeField, Range(0f, 1f)] private float slideHeight = 0.5f;
    [SerializeField] private float slideTiltAmount = 5f;
    [SerializeField] private float slideCancelDelay = 0.1f;
    [SerializeField] private float maximumHeightOffset = -1.5f;
    [SerializeField] private Transform cameraOffset;
    private float originalCameraPosY = 0.5f;
    private Vector3 initialSlideDirection;
    private bool canCancelSlide = false;
    private Timer slideCancelTimer;
    private float originalCapsuleHeight = 0f;
    private float originalCapsuleCenterY = 0f;

    [Header("Slope and step specifics")]
    public float groundCheckerThrashold = 0.1f;
    public float maxGroundDistance = 0.1f;
    public float groundStickSmooth = 2f;
    public float slopeCheckerThrashold = 0.51f;
    public float stepCheckerThrashold = 0.6f;
    [Space(2)]

    [Range(1f, 89f)]
    public float maxClimbableSlopeAngle = 53.6f;
    public float maxStepHeight = 0.74f;
    public float stepSmooth = 1f;
    [SerializeField] private Transform stepLowerTransform;
    [SerializeField] private Transform groundCheckTransform;
    [Space(2)]

    public AnimationCurve speedMultiplierOnAngle = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Range(0.01f, 1f)]
    public float canSlideMultiplierCurve = 0.061f;
    [Range(0.01f, 1f)]
    public float cantSlideMultiplierCurve = 0.039f;
    [Range(0.01f, 1f)]
    public float climbingStairsMultiplierCurve = 0.637f;
    [Space(2)]

    public float gravityMultiplier = 6f;
    public float gravityMultiplyerOnSlideChange = 3f;
    public float gravityMultiplierIfUnclimbableSlope = 30f;
    [Space(2)]

    public bool lockOnSlope = false;

    [Header("Sprint and crouch specifics")]
    public float sprintSpeed = 20f;

    private Vector3 forward;
    private Vector3 globalForward;
    private Vector3 reactionForward;
    private Vector3 down;
    private Vector3 globalDown;
    private Vector3 reactionGlobalDown;

    public bool debug = true;

    public float minimumVerticalSpeedToLandEvent;

    public float minimumHorizontalSpeedToFastEvent;

    private float currentSurfaceAngle;
    private bool currentLockOnSlope;


    private Vector3 wallNormal;
    private Vector3 groundNormal;
    private Vector3 prevGroundNormal;
    private bool prevGrounded;

    private float coyoteJumpMultiplier = 1f;

    public bool isGrounded = false;
    public bool isTouchingSlope = false;
    public bool isTouchingStep = false;
    public bool isTouchingWall = false;

    public bool isMoving = false;
    public bool isJumping = false;
    public bool isSliding = false;
    public bool isCrouch = false;
    public bool isSprinting = false;
    public bool isFalling = false;
    public bool shouldDoFootIk = true;

    private Vector2 axisInput;

    [HideInInspector]
    public float targetAngle;
    private Rigidbody rigidbody;
    private CapsuleCollider collider;
    private PlayerController playerController;
    private float originalColliderHeight;

    private Vector3 currVelocity = Vector3.zero;
    private float turnSmoothVelocity;
    private bool lockRotation = false;
    private bool lockToCamera = false;
    private Timer jumpTimer;
    private Vector3 stepHit;
    private RaycastHit groundHit;
    private float height1 = 0f; // for falling
    private float height2 = 0f; // for falling

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        playerController = GetComponent<PlayerController>();
        originalColliderHeight = collider.height;

        jumpTimer = gameObject.AddComponent<Timer>();
        slideCancelTimer = gameObject.AddComponent<Timer>();
        dashTimer = gameObject.AddComponent<Timer>();
        dashCoolDownTimer = gameObject.AddComponent<Timer>();

        SetFriction(frictionAgainstFloor, true);
        currentLockOnSlope = lockOnSlope;
        originalCameraPosY = cameraOffset.localPosition.y;

        InputManager.Instance.playerInput.InGame.Jump.started += _ctx => Jump();
        InputManager.Instance.playerInput.InGame.Slide.started += _ctx => StartSliding();
        InputManager.Instance.playerInput.InGame.Slide.canceled += _ctx => EndSlide();
    }

    private void FixedUpdate()
    {
        //local vectors
        CheckGrounded();
        CheckStep();
        CheckWall();
        CheckSlopeAndDirections();
        CheckFalling();

        //movement
        if (isSliding)
        {
            MoveSliding();
        }
        else
        {
            MoveWalk();
        }
        if (!isDashing && !isMoving)
        {
            ApplyDecceleration();
        }

        //gravity
        ApplyGravity();

        UiManager.Instance.playerHud.UpdateSpeedText(GetHorizontalVelocity().magnitude);
    }

    private void ApplyDecceleration()
    {
        Vector3 currentVelocity = rigidbody.velocity;

        // Stop horizontal movement while retaining vertical velocity
        Vector3 newVelocity = new Vector3(0, currentVelocity.y, 0);

        float currentDecceleration = isGrounded ? decceleration : airDecceleration;

        // Dynamically adjust smoothing time for stopping as well
        float smoothingTime = Mathf.Max(0.01f, 1f / currentDecceleration);

        rigidbody.velocity = Vector3.SmoothDamp(
            currentVelocity,
            newVelocity,
            ref currVelocity,
            smoothingTime
        );
    }

    private void CheckFalling()
    {
        if (prevGrounded && !isGrounded)
        {
            // We are now in the are
            height1 = transform.position.y;
        }
        else if (!prevGrounded && isGrounded)
        {
            // We have landed
            height2 = transform.position.y;
            playerController.playerLook.PlayLandAnimation();
            remainingWallJumps = maxWallJumps;
            ResetVerticalVelocity();

            if (height1 - height2 > 2)
            {
                Debug.Log("We fell a long distance do add functionality");
            }
        }
    }


    #region Checks
    private void CheckGrounded()
    {
        prevGrounded = isGrounded;
        bool tempGrounded = false;
        // Define the positions of the corners relative to groundCheckPosition.position
        Vector3[] cornerOffsets = new Vector3[]
        {
            new Vector3(-groundCheckerThrashold/2, 0, 0), // Left
            new Vector3(groundCheckerThrashold/2, 0, 0),  // Right
            new Vector3(0, 0, groundCheckerThrashold/2),  // Front
            new Vector3(0, 0, -groundCheckerThrashold/2)  // Back
        };
        // Perform raycasts from each corner
        foreach (Vector3 offset in cornerOffsets)
        {
            if (Physics.Raycast(groundCheckTransform.position + offset, Vector3.down, out groundHit, maxGroundDistance, groundMask))
            {
                tempGrounded = true;
                break;
            }
        }

        isGrounded = tempGrounded;
        if (isGrounded && !isFalling && !isJumping && !isTouchingSlope && !isDashing)
        {
            Vector3 targetPosition = new Vector3(rigidbody.position.x, groundHit.point.y, rigidbody.position.z);
            rigidbody.position = Vector3.Lerp(rigidbody.position, targetPosition, groundStickSmooth * Time.fixedDeltaTime);
        }
    }


    private void CheckStep()
    {
        bool tmpStep = false;

        RaycastHit stepLowerHit;
        if (Physics.Raycast(stepLowerTransform.position, globalForward, out stepLowerHit, stepCheckerThrashold, groundMask))
        {
            RaycastHit stepUpperHit;
            if (RoundValue(stepLowerHit.normal.y) == 0 && !Physics.Raycast(stepLowerTransform.position + new Vector3(0f, maxStepHeight, 0f), globalForward, out stepUpperHit, stepCheckerThrashold + 0.05f, groundMask))
            {
                //rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
                tmpStep = true;
                stepHit = stepLowerHit.point;
            }
        }

        RaycastHit stepLowerHit45;
        if (Physics.Raycast(stepLowerTransform.position, Quaternion.AngleAxis(45, transform.up) * globalForward, out stepLowerHit45, stepCheckerThrashold, groundMask))
        {
            RaycastHit stepUpperHit45;
            if (RoundValue(stepLowerHit45.normal.y) == 0 && !Physics.Raycast(stepLowerTransform.position + new Vector3(0f, maxStepHeight, 0f), Quaternion.AngleAxis(45, Vector3.up) * globalForward, out stepUpperHit45, stepCheckerThrashold + 0.05f, groundMask))
            {
                //rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
                tmpStep = true;
                stepHit = stepLowerHit45.point;
            }
        }

        RaycastHit stepLowerHitMinus45;
        if (Physics.Raycast(stepLowerTransform.position, Quaternion.AngleAxis(-45, transform.up) * globalForward, out stepLowerHitMinus45, stepCheckerThrashold, groundMask))
        {
            RaycastHit stepUpperHitMinus45;
            if (RoundValue(stepLowerHitMinus45.normal.y) == 0 && !Physics.Raycast(stepLowerTransform.position + new Vector3(0f, maxStepHeight, 0f), Quaternion.AngleAxis(-45, Vector3.up) * globalForward, out stepUpperHitMinus45, stepCheckerThrashold + 0.05f, groundMask))
            {
                //rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
                tmpStep = true;
                stepHit = stepLowerHitMinus45.point;
            }
        }

        isTouchingStep = tmpStep;

        if (isGrounded && !isFalling && !isJumping && !isTouchingSlope && !isDashing)
        {
            if (isTouchingStep && isMoving)
            {
                Vector3 targetPosition = new Vector3(rigidbody.position.x, stepHit.y, rigidbody.position.z);
                rigidbody.position = Vector3.Lerp(rigidbody.position, targetPosition, stepSmooth * Time.fixedDeltaTime);
                ResetVerticalVelocity();
            }
        }
        else if (!isFalling && !isJumping && !isTouchingSlope && !isDashing)
        {
            if (isTouchingStep && isMoving)
            {
                Vector3 targetPosition = new Vector3(rigidbody.position.x, stepHit.y, rigidbody.position.z);
                rigidbody.position = Vector3.Lerp(rigidbody.position, targetPosition, stepSmooth * 2 * Time.fixedDeltaTime);
                ResetVerticalVelocity();
            }
        }
    }


    private void CheckWall()
    {
        bool tmpWall = false;
        Vector3 tmpWallNormal = Vector3.zero;
        Vector3 topWallPos = new Vector3(transform.position.x, transform.position.y + hightWallCheckerChecker, transform.position.z);

        RaycastHit wallHit;
        if (Physics.Raycast(topWallPos, globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }
        else if (Physics.Raycast(topWallPos, Quaternion.AngleAxis(45, transform.up) * globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }
        else if (Physics.Raycast(topWallPos, Quaternion.AngleAxis(90, transform.up) * globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }
        else if (Physics.Raycast(topWallPos, Quaternion.AngleAxis(135, transform.up) * globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }
        else if (Physics.Raycast(topWallPos, Quaternion.AngleAxis(180, transform.up) * globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }
        else if (Physics.Raycast(topWallPos, Quaternion.AngleAxis(225, transform.up) * globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }
        else if (Physics.Raycast(topWallPos, Quaternion.AngleAxis(270, transform.up) * globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }
        else if (Physics.Raycast(topWallPos, Quaternion.AngleAxis(315, transform.up) * globalForward, out wallHit, wallCheckerThrashold, groundMask))
        {
            tmpWallNormal = wallHit.normal;
            tmpWall = true;
        }

        isTouchingWall = tmpWall;
        wallNormal = tmpWallNormal;
    }

    private Vector3 GetHorizontalVelocity()
    {
        return new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
    }

    private void CheckSlopeAndDirections()
    {
        if (isJumping || !isGrounded || isDashing)
        {
            // Airborne logic
            forward = transform.forward; // Keep the forward direction as the character's facing direction
            Vector3 horizontalVelocity = GetHorizontalVelocity();
            if (horizontalVelocity.magnitude > 0)
            {
                // keeps last moving direction as forward if not moving
                globalForward = horizontalVelocity.normalized;
            }
            reactionForward = forward;

            // Set default down directions
            down = Vector3.down.normalized;
            globalDown = Vector3.down.normalized;
            reactionGlobalDown = Vector3.up.normalized;

            // Avoid applying ground friction
            SetFriction(0f, false);
            currentLockOnSlope = false;
            isTouchingSlope = false;

            return; // Exit the method to skip ground-related checks
        }

        prevGroundNormal = groundNormal;

        RaycastHit slopeHit;
        if (Physics.SphereCast(groundCheckTransform.position, slopeCheckerThrashold, Vector3.down, out slopeHit, groundMask))
        {
            groundNormal = slopeHit.normal;

            if (slopeHit.normal.y > 0.99f && slopeHit.normal.y < 1.01f)
            {
                forward = transform.forward;
                globalForward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).normalized;
                reactionForward = forward;

                SetFriction(frictionAgainstFloor, true);
                currentLockOnSlope = lockOnSlope;

                currentSurfaceAngle = 0f;
                isTouchingSlope = false;
            }
            else
            {
                Vector3 tmpGlobalForward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).normalized;
                Vector3 tmpForward = Vector3.ProjectOnPlane(transform.forward.normalized, slopeHit.normal);
                Vector3 tmpReactionForward = new Vector3(tmpForward.x, tmpGlobalForward.y - tmpForward.y, tmpForward.z);

                float speedMultiplier = speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f);

                if (currentSurfaceAngle <= maxClimbableSlopeAngle && !isTouchingStep)
                {
                    forward = tmpForward * (speedMultiplier * canSlideMultiplierCurve + 1f);
                    globalForward = tmpGlobalForward * (speedMultiplier * canSlideMultiplierCurve + 1f);
                    reactionForward = tmpReactionForward * (speedMultiplier * canSlideMultiplierCurve + 1f);

                    SetFriction(frictionAgainstFloor, true);
                    currentLockOnSlope = lockOnSlope;
                }
                else if (isTouchingStep)
                {
                    forward = tmpForward * (speedMultiplier * climbingStairsMultiplierCurve + 1f);
                    globalForward = tmpGlobalForward * (speedMultiplier * climbingStairsMultiplierCurve + 1f);
                    reactionForward = tmpReactionForward * (speedMultiplier * climbingStairsMultiplierCurve + 1f);

                    SetFriction(frictionAgainstFloor, true);
                    currentLockOnSlope = true;
                }
                else
                {
                    forward = tmpForward * (speedMultiplier * cantSlideMultiplierCurve + 1f);
                    globalForward = tmpGlobalForward * (speedMultiplier * cantSlideMultiplierCurve + 1f);
                    reactionForward = tmpReactionForward * (speedMultiplier * cantSlideMultiplierCurve + 1f);

                    SetFriction(0f, true);
                    currentLockOnSlope = lockOnSlope;
                }

                currentSurfaceAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                isTouchingSlope = true;
            }

            down = Vector3.Project(Vector3.down, slopeHit.normal);
            globalDown = Vector3.down.normalized;
            reactionGlobalDown = Vector3.up.normalized;
        }
        else
        {
            groundNormal = Vector3.zero;

            forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            globalForward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).normalized;
            reactionForward = forward;

            down = Vector3.down.normalized;
            globalDown = Vector3.down.normalized;
            reactionGlobalDown = Vector3.up.normalized;

            SetFriction(frictionAgainstFloor, true);
            currentLockOnSlope = lockOnSlope;
        }
    }

    #endregion


    private void Jump()
    {
        if (isJumping) return;

        if (isGrounded)
        {
            rigidbody.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
            isJumping = true;

            jumpTimer.SetTimer(jumpTime, JumpEnd);
        }
        else if (isTouchingWall && remainingWallJumps > 0)
        {
            remainingWallJumps--;

            rigidbody.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
            rigidbody.AddForce(wallNormal * jumpVelocity, ForceMode.VelocityChange);
            isJumping = true;

            jumpTimer.SetTimer(jumpTime, JumpEnd);
        }
    }

    private void JumpEnd()
    {
        isJumping = false;
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

            axisInput = InputManager.Instance.MovementVector;

            // If input detected then apply it
            if (axisInput.sqrMagnitude > Mathf.Epsilon && !ignoreInput)
            {
                dashDirection = new Vector3(axisInput.x, 0, axisInput.y);
                dashDirection.Normalize();
                dashDirection = transform.TransformDirection(dashDirection);
            }

            ResetVerticalVelocity();
            ResetHorizontalVelocity();
            rigidbody.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

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
    }

    void RefreshDash()
    {
        canDash = true;
    }

    private void StartSliding()
    {
        if (isGrounded)
        {
            float characterHeightOffset = -originalCameraPosY * (1 - slideHeight);

            //To do: offset the collider and shrink it
            collider.height += characterHeightOffset;
            collider.center = new Vector3(collider.center.x,
                                                 collider.center.y + (characterHeightOffset / 2),
                                                 collider.center.z);

            cameraOffset.transform.localPosition = new Vector3(0, originalCameraPosY + characterHeightOffset, 0);

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
        }
    }

    private void EndSlide()
    {
        if (isSliding)
        {
            float characterHeightOffset = maximumHeightOffset * (1 - slideHeight);

            collider.height -= characterHeightOffset;
            collider.center = new Vector3(collider.center.x,
                                          collider.center.y - (characterHeightOffset / 2),
                                          collider.center.z);

            cameraOffset.transform.localPosition = new Vector3(0, originalCameraPosY, 0);

            // Set the sliding state to false
            isSliding = false;

            // Restore the original camera tilt amount
            playerController.playerLook.tiltAmount = playerController.playerLook.originalTiltAmount;
        }
    }

    private void ResetVerticalVelocity()
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
    }

    private void ResetHorizontalVelocity()
    {
        rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
    }

    #region Move

    private void MoveCrouch()
    {
        //if (crouch && isGrounded)
        //{
        //    isCrouch = true;
        //    if (meshCharacterCrouch != null && meshCharacter != null) meshCharacter.SetActive(false);
        //    if (meshCharacterCrouch != null) meshCharacterCrouch.SetActive(true);
        //
        //    float newHeight = originalColliderHeight * crouchHeightMultiplier;
        //    collider.height = newHeight;
        //    collider.center = new Vector3(0f, -newHeight * crouchHeightMultiplier, 0f);
        //
        //    headPoint.position = new Vector3(transform.position.x + POV_crouchHeadHeight.x, transform.position.y + POV_crouchHeadHeight.y, transform.position.z + POV_crouchHeadHeight.z);
        //}
        //else
        //{
        //    isCrouch = false;
        //    if (meshCharacterCrouch != null && meshCharacter != null) meshCharacter.SetActive(true);
        //    if (meshCharacterCrouch != null) meshCharacterCrouch.SetActive(false);
        //
        //    collider.height = originalColliderHeight;
        //    collider.center = Vector3.zero;
        //
        //    headPoint.position = new Vector3(transform.position.x + POV_normalHeadHeight.x, transform.position.y + POV_normalHeadHeight.y, transform.position.z + POV_normalHeadHeight.z);
        //}
    }


    private void MoveWalk()
    {
        float crouchMultiplier = 1f;
        if (isCrouch) crouchMultiplier = crouchSpeedMultiplier;

        Vector3 currentVelocity = rigidbody.velocity;

        axisInput = InputManager.Instance.MovementVector;

        if (axisInput.magnitude > movementThrashold)
        {
            Vector3 forwardMovement = transform.forward * axisInput.y; // Forward/backward
            Vector3 rightMovement = transform.right * axisInput.x; // Sideways (strafe)
            Vector3 direction = (forwardMovement + rightMovement).normalized;

            float speed = isSprinting ? sprintSpeed : movementSpeed;

            // Calculate target velocity
            Vector3 targetVelocity = new Vector3(
                direction.x * speed * crouchMultiplier,
                currentVelocity.y,
                direction.z * speed * crouchMultiplier
            );

            // If current velocity is greater than target velocity, scale down gradually
            if (currentVelocity.magnitude > speed * crouchMultiplier)
            {
                // Allow player input to influence direction, but scale the velocity down
                Vector3 velocityAdjustment = Vector3.Lerp(
                    currentVelocity,
                    targetVelocity,
                    Time.deltaTime * slowDownDecceleration
                );
                targetVelocity = velocityAdjustment;
            }


            // Dynamically adjust smoothing time based on acceleration
            float currentAcceleration = isGrounded ? acceleration : airAcceleration;
            float smoothingTime = Mathf.Max(0.01f, 1f / currentAcceleration);

            // Apply smoothing for movement
            rigidbody.velocity = Vector3.SmoothDamp(
                currentVelocity,
                targetVelocity,
                ref currVelocity,
                smoothingTime
            );
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    private void MoveSliding()
    {
        Vector3 currentVelocity = rigidbody.velocity;

        Vector3 slideDirectionRight = Vector3.Cross(Vector3.up, initialSlideDirection);

        Vector3 blendedDirection = initialSlideDirection + (InputManager.Instance.MovementVector.x * slideDirectionRight * slideHorizontalMovement);
        blendedDirection.Normalize();

        // Retain vertical velocity for gravity or stepping
        Vector3 newVelocity = new Vector3(
            blendedDirection.x * slideSpeed,
            currentVelocity.y,
            blendedDirection.z * slideSpeed
        );

        float smoothingTime = Mathf.Max(0.01f, 1f / slideAcceleration);

        // Apply smoothing for movement
        rigidbody.velocity = Vector3.SmoothDamp(
            currentVelocity,
            newVelocity,
            ref currVelocity,
            smoothingTime
        );
    }


    #endregion


    #region Gravity

    private void ApplyGravity()
    {
        if (isGrounded) return;

        Vector3 gravity = Vector3.zero;

        if (currentLockOnSlope || isTouchingStep)
        {
            gravity = coyoteJumpMultiplier * gravityMultiplier * -Physics.gravity.y * down;
        }
        else
        {
            gravity = coyoteJumpMultiplier * gravityMultiplier * -Physics.gravity.y * globalDown;
        }

        //avoid little jump
        if (groundNormal.y != 1 && groundNormal.y != 0 && isTouchingSlope && prevGroundNormal != groundNormal)
        {
            //Debug.Log("Added correction jump on slope");
            gravity *= gravityMultiplyerOnSlideChange;
        }

        //slide if angle too big
        if (groundNormal.y != 1 && groundNormal.y != 0 && (currentSurfaceAngle > maxClimbableSlopeAngle && !isTouchingStep))
        {
            //Debug.Log("Slope angle too high, character is sliding");
            if (currentSurfaceAngle > 0f && currentSurfaceAngle <= 30f) gravity = globalDown * gravityMultiplierIfUnclimbableSlope * -Physics.gravity.y;
            else if (currentSurfaceAngle > 30f && currentSurfaceAngle <= 89f) gravity = globalDown * gravityMultiplierIfUnclimbableSlope / 2f * -Physics.gravity.y;
        }

        //friction when touching wall
        if (isTouchingWall && rigidbody.velocity.y < 0) gravity *= frictionAgainstWall;

        rigidbody.AddForce(gravity);
    }

    #endregion


    //private void FootTrace(Transform footBone, Transform rootTransform, out float yOffset, out Quaternion surfaceRotationOffset)
    //{
    //    // Start position for the sphere cast in world space
    //    Vector3 startPosition = footBone.position + Vector3.up * 0.5f;
    //
    //    RaycastHit hit;
    //    if (Physics.SphereCast(startPosition, footTraceRadius, Vector3.down, out hit, castDistance, groundMask))
    //    {
    //        // Convert the hit point and foot position to local space
    //        Vector3 localHitPoint = rootTransform.InverseTransformPoint(hit.point);
    //        Vector3 localFootPosition = rootTransform.InverseTransformPoint(footBone.position);
    //
    //        // Calculate the vertical offset in local space
    //        yOffset = localHitPoint.y - localFootPosition.y;
    //
    //        // Calculate surface rotation offset in local space
    //        surfaceRotationOffset = Quaternion.FromToRotation(Vector3.up, hit.normal);
    //
    //        // Debug Visualization
    //        Debug.DrawLine(startPosition, hit.point, Color.blue); // Ray to hit point
    //        Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.green); // Surface normal
    //        DrawDisc(hit.point, hit.normal, footTraceRadius, Color.yellow); // Visualize the contact disc
    //    }
    //    else
    //    {
    //        // No hit: Default Y-offset to zero, rotation remains unchanged
    //        yOffset = 0;
    //        surfaceRotationOffset = Quaternion.identity;
    //
    //        // Debug Visualization for missed cast
    //        Debug.DrawLine(startPosition, startPosition + Vector3.down * castDistance, Color.red);
    //    }
    //}




    #region Friction and Round

    private void SetFriction(float _frictionWall, bool _isMinimum)
    {
        collider.material.dynamicFriction = 0.6f * _frictionWall;
        collider.material.staticFriction = 0.6f * _frictionWall;
        
        if (_isMinimum) collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
        else collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
    }


    private float RoundValue(float _value)
    {
        float unit = (float)Mathf.Round(_value);

        if (_value - unit < 0.000001f && _value - unit > -0.000001f) return unit;
        else return _value;
    }

    #endregion


    #region GettersSetters

    public bool GetGrounded() { return isGrounded; }
    public bool GetTouchingSlope() { return isTouchingSlope; }
    public bool GetTouchingStep() { return isTouchingStep; }
    public bool GetTouchingWall() { return isTouchingWall; }
    public bool GetJumping() { return isJumping; }
    public bool GetCrouching() { return isCrouch; }
    public float GetOriginalColliderHeight() { return originalColliderHeight; }

    public void SetLockRotation(bool _lock) { lockRotation = _lock; }

    #endregion


    #region Gizmos

    private void OnDrawGizmos()
    {
        if (debug)
        {
            rigidbody = this.GetComponent<Rigidbody>();
            collider = this.GetComponent<CapsuleCollider>();

            Vector3 bottomStepPos = stepLowerTransform.position;
            Vector3 topWallPos = new Vector3(transform.position.x, transform.position.y + hightWallCheckerChecker, transform.position.z);

            // Define the positions of the corners relative to groundCheckPosition.position
            Vector3[] cornerOffsets = new Vector3[]
            {
            new Vector3(-groundCheckerThrashold/2, 0, 0), // Left
            new Vector3(groundCheckerThrashold/2, 0, 0),  // Right
            new Vector3(0, 0, groundCheckerThrashold/2),  // Front
            new Vector3(0, 0, -groundCheckerThrashold/2)  // Back
            };


            // Draw lines between the corners to form the 2D box
            Gizmos.DrawLine(groundCheckTransform.position + cornerOffsets[0], groundCheckTransform.position + cornerOffsets[2]);
            Gizmos.DrawLine(groundCheckTransform.position + cornerOffsets[2], groundCheckTransform.position + cornerOffsets[1]);
            Gizmos.DrawLine(groundCheckTransform.position + cornerOffsets[1], groundCheckTransform.position + cornerOffsets[3]);
            Gizmos.DrawLine(groundCheckTransform.position + cornerOffsets[3], groundCheckTransform.position + cornerOffsets[0]);

            // Draw the ground check rays
            Gizmos.color = Color.blue;
            foreach (Vector3 offset in cornerOffsets)
            {
                Gizmos.DrawRay(groundCheckTransform.position + offset, Vector3.down * maxGroundDistance);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckTransform.position, slopeCheckerThrashold);

            //direction
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + forward * 2f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + globalForward * 2);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + reactionForward * 2f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + down * 2f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + globalDown * 2f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + reactionGlobalDown * 2f);

            //step check
            Gizmos.color = Color.black;
            Gizmos.DrawLine(bottomStepPos, bottomStepPos + globalForward * stepCheckerThrashold);

            Gizmos.color = Color.black;
            Gizmos.DrawLine(bottomStepPos + new Vector3(0f, maxStepHeight, 0f), bottomStepPos + new Vector3(0f, maxStepHeight, 0f) + globalForward * (stepCheckerThrashold + 0.05f));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(bottomStepPos, bottomStepPos + Quaternion.AngleAxis(45, transform.up) * (globalForward * stepCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(bottomStepPos + new Vector3(0f, maxStepHeight, 0f), bottomStepPos + Quaternion.AngleAxis(45, Vector3.up) * (globalForward * stepCheckerThrashold) + new Vector3(0f, maxStepHeight, 0f));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(bottomStepPos, bottomStepPos + Quaternion.AngleAxis(-45, transform.up) * (globalForward * stepCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(bottomStepPos + new Vector3(0f, maxStepHeight, 0f), bottomStepPos + Quaternion.AngleAxis(-45, Vector3.up) * (globalForward * stepCheckerThrashold) + new Vector3(0f, maxStepHeight, 0f));

            //wall check
            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + globalForward * wallCheckerThrashold);

            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + Quaternion.AngleAxis(45, transform.up) * (globalForward * wallCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + Quaternion.AngleAxis(90, transform.up) * (globalForward * wallCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + Quaternion.AngleAxis(135, transform.up) * (globalForward * wallCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + Quaternion.AngleAxis(180, transform.up) * (globalForward * wallCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + Quaternion.AngleAxis(225, transform.up) * (globalForward * wallCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + Quaternion.AngleAxis(270, transform.up) * (globalForward * wallCheckerThrashold));

            Gizmos.color = Color.black;
            Gizmos.DrawLine(topWallPos, topWallPos + Quaternion.AngleAxis(315, transform.up) * (globalForward * wallCheckerThrashold));
        }
    }

    #endregion
}