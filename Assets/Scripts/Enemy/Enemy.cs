using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IDamageable
{
    protected Rigidbody rb;
    [HideInInspector] public NavMeshAgent agent;
    public Animator animator;
    [HideInInspector] public Transform target;
    public EnemyStateMachine StateMachine;
    [Header("Settings")]
    public float updatePathTime = 1.0f;
    public float lookDistance = 30f;
    public float fieldOfViewAngle = 110f;
    public bool isRanged = false;
    public float attackDistance = 0.1f;
    public float attackStartDelay = 0.1f;
    public float loseDistance = 5f;
    public float attackDuration = 1.0f;

    [HideInInspector] public Timer delayTimer;
    [HideInInspector] public Weapon[] weapons;
    [HideInInspector] public bool canRotate = true;
    protected SpriteRenderer[] spriteRenderers;

    [Header("Shooting Settings")]
    public int bulletsPerBurst = 3;
    public float burstDelay = 0.5f;
    public float minRadius = 1f;
    public float maxRadius = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.1f;
    public bool isDashing = false;
    private Vector3 dashDirection;
    private Vector3 dashStartPosition;
    public LayerMask obstacleLayer;

    [Header("Launch Settings")]
    protected Timer launchTimer;
    public bool isLaunching = false;
    [SerializeField] protected float launchbackThreshold = 30.0f;

    [Header("Health")]
    public bool isInvinsable = false;
    public float maxHealth = 100;
    protected float health;

    [Header("Ground Settings")]
    public bool isGrounded = true;
    private float groundCheckDistance = 0.1f;
    private LayerMask groundLayer;

    [SerializeField] protected GameObject deathParticles;

    public float Health
    {
        get => health;
        set
        {
            if (isInvinsable) return;
            OnHealthChanged?.Invoke();
            health = value;

            if (health <= 0)
            {
                Die();
            }
        }
    }
    public event Action OnHealthChanged;

    public event Action OnDeath;

    [Header("State")]
    [SerializeField] protected EnemyState defaultState = EnemyState.Looking;
    [SerializeField] protected EnemyState currentState;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        health = maxHealth;

        StateMachine = new EnemyStateMachineBuilder()
            .AddState(new LookingState())
            .AddState(new ChaseState())
            .AddState(new AttackingState())
            .Build();

        SetDefaultState();
        delayTimer = gameObject.AddComponent<Timer>();
        launchTimer = gameObject.AddComponent<Timer>();
        weapons = GetComponentsInChildren<Weapon>();
        groundLayer = LayerMask.GetMask("Ground");
    }

    protected void Update()
    {
        StateMachine.Update(this);
        currentState = StateMachine.GetCurrentState(); // Update currentState for display
        if (canRotate)
        {
            LookTowardsTarget();
        }
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDashMovement();
        }
        else if (!agent.enabled)
        {
            CheckGrounded();
        }
    }

    protected void SetDefaultState()
    {
        switch (defaultState)
        {
            case EnemyState.Looking:
                StateMachine.ChangeState(new LookingState(), this);
                break;
            case EnemyState.Chasing:
                StateMachine.ChangeState(new ChaseState(), this);
                break;
            case EnemyState.Attacking:
                StateMachine.ChangeState(new AttackingState(), this);
                break;
            case EnemyState.BossAttacking:
                StateMachine.ChangeState(new BossAttackingState(), this);
                break;
            case EnemyState.FlyingWonder:
                StateMachine.ChangeState(new FlyingWonderState(), this);
                break;
            case EnemyState.FlyingAttacking:
                StateMachine.ChangeState(new FlyingAttackingState(), this);
                break;
            case EnemyState.Idle:
                StateMachine.ChangeState(new IdleState(), this);
                break;
        }
    }

    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
        StartCoroutine(FlashRed());

        //if (health > 0)
        //{
        //    isLaunching = true;
        //    StartCoroutine(LaunchUpwards(damageAmount));
        //}
    }

    protected IEnumerator LaunchUpwards(float damageAmount)
    {
        yield return null;

        float upwardForce = Mathf.Clamp(damageAmount, 0, launchbackThreshold);
        
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.y <= 0.01f);

        EndLaunch();
    }

    protected void EndLaunch()
    {
        agent.enabled = true;
        isLaunching = false;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void Heal(float healAmount)
    {
        float newHealth = Health + healAmount;
        Health = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    public virtual void Die()
    {
        InvokeOnDeath();
        foreach (Weapon weapon in weapons)
        {
            weapon.Throw(Vector3.up, 0.0f, 0.0f);
        }
        Instantiate(deathParticles, transform.position + new Vector3(0.0f, 1.5f, 0.0f), Quaternion.identity);
        Destroy(gameObject);
    }
    protected void InvokeOnDeath()
    {
        OnDeath?.Invoke();
    }

    public void SetTarget(Transform Target)
    {
        target = Target;
    }

    public bool IsTargetInFOV()
    {
        if (target == null)
            return false;

        Vector3 directionToTarget = target.position - transform.position;
        float angleToTarget = Vector3.Angle(directionToTarget, transform.forward);

        if (angleToTarget < fieldOfViewAngle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget.normalized, out hit, lookDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void StartAttack()
    {
        if (isRanged)
        {
            foreach (Weapon weapon in weapons)
            {
                weapon.StartShooting();
            }
            if(weapons.Length == 0)
            {
                Debug.LogAssertion("Enemy has no attached Weapon but is trying to shoot a weapon");
            }
        }
        else
        {
            animator.SetTrigger("Attack");
        }
    }

    public virtual void EndAttack()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.EndShooting();
        }
    }

    public void Dash(float _dashSpeed = 0, float _dashDuration = 0, Vector3 _dashDirection = default)
    {
        if (_dashDirection == default)
        {
            _dashDirection = transform.forward;
        }
        dashSpeed = _dashSpeed == 0 ? dashSpeed : _dashSpeed;
        dashDuration = _dashDuration == 0 ? dashDuration : _dashDuration;
        dashDirection = _dashDirection;

        if (!isDashing)
        {
            isDashing = true;
            agent.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = false;
            dashStartPosition = transform.position;
            delayTimer.startTime = Time.time;
            // Start the dash
            HandleDashMovement();

            // Set timer to stop the dash
            delayTimer.SetTimer(dashDuration, StopDash);
        }
    }

    private void HandleDashMovement()
    {
        if (isDashing)
        {
            float currentSpeed = CalculateCurrentSpeed();
            rb.AddForce(dashDirection * currentSpeed *0.1f, ForceMode.Impulse);

            // Check for collisions
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dashDirection, out hit, dashSpeed * Time.fixedDeltaTime, obstacleLayer))
            {
                AdjustForCollision(hit.normal);
            }

            // Check if dash duration has expired
            if (Time.time >= delayTimer.startTime + dashDuration)
            {
                StopDash();
            }
        }
        CheckGrounded();
    }

    private float CalculateCurrentSpeed()
    {
        float elapsedTime = Time.timeSinceLevelLoad - delayTimer.startTime;
        float t = elapsedTime / dashDuration;
        return Mathf.Lerp(dashSpeed, 0f, t * t); // Quadratic slowdown
    }

    private void AdjustForCollision(Vector3 normal)
    {
        // Calculate sliding direction
        Vector3 slideDir = Vector3.ProjectOnPlane(dashDirection, normal).normalized;

        // Update dash direction
        dashDirection = slideDir;
    }

    private void StopDash()
    {
        Debug.Log("stop dash");
        isDashing = false;
        rb.useGravity = true;
        rb.isKinematic = false;

        // Only enable agent if grounded
        if (isGrounded)
        {
            EnableAgent();
        }
    }

    private void EnableAgent()
    {
        agent.enabled = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
    private void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            if (!isGrounded)
            {
                isGrounded = true;
                EnableAgent();
            }
        }
        else
        {
            isGrounded = false;
        }
    }
    private void LookTowardsTarget()
    {
        if (target != null)
        {
            // Determine direction to look at
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0; // Keep only the horizontal direction

            // Determine the rotation needed to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly rotate the enemy towards the target
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 1000.0f
            );
        }
    }
    private IEnumerator FlashRed()
    {
        foreach (var sr in spriteRenderers)
        {
            if(sr.GetComponent<Weapon>() != null) { continue; };
            sr.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var sr in spriteRenderers)
        {
            sr.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        OnHealthChanged = null;
        OnDeath = null;
    }
}