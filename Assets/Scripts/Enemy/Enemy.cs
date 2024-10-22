using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [HideInInspector] public NavMeshAgent agent;
    public Animator animator;
    [HideInInspector] public Transform target;
    public EnemyStateMachine StateMachine;
    private Vector3 dashDirection;
    [Header("Settings")]
    public float updatePathTime = 1.0f; // time to update the path towards the target
    public float lookDistance = 30f;
    public float fieldOfViewAngle = 110f;
    public bool isRanged = false;
    public float attackDistance = 0.1f;
    public float attackStartDelay = 0.1f;
    public float loseDistance = 5f;
    public float attackDuration = 1.0f;
    public float rotationFreezeTime = 0.4f;
    public float attackResumeRotationDelay = 0.01f;

    [HideInInspector] public Timer delayTimer;
    [HideInInspector] public Weapon weapon;
    [HideInInspector] public bool canRotate = true;
    private SpriteRenderer[] spriteRenderers;

    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.1f;
    public bool isDashing = false;

    [Header("Health")]
    public int maxHealth = 100;
    private float health;
    public float Health
    {
        get => health;
        set
        {
            health = value;

            if (health <= 0)
            {
                Die();
            }
        }
    }

    public event Action OnDeath;

    [Header("State")]
    [SerializeField] private EnemyState defaultState = EnemyState.Looking;
    [SerializeField] private EnemyState currentState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Health = maxHealth;

        StateMachine = new EnemyStateMachineBuilder()
            .AddState(new LookingState())
            .AddState(new ChaseState())
            .AddState(new AttackingState())
            .Build();

        SetDefaultState();
        delayTimer = gameObject.AddComponent<Timer>();
        weapon = GetComponentInChildren<Weapon>();
    }

    private void Update()
    {
        StateMachine.Update(this);
        currentState = StateMachine.GetCurrentState(); // Update currentState for display
        if(canRotate)
        {
            LookTowardsTarget();
        }
        if (isDashing)
        {
            agent.Move(dashDirection * dashSpeed * Time.deltaTime);
        }
    }

    private void SetDefaultState()
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
        }
    }

    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
        StartCoroutine(FlashRed());
        
    }


    public void Heal(float healAmount)
    {
        float newHealth = Health + healAmount;
        Health = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        if (weapon == null)
        {
            weapon = GetComponentInChildren<Weapon>();
        }

        if (weapon != null)
        {
            weapon.Throw(Vector3.up, 0.0f, 0.0f);
        }

        Destroy(gameObject);
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
            if (weapon == null)
            {
                weapon = GetComponentInChildren<Weapon>();
            }
            if (weapon != null)
            {
                weapon.predictionTime = rotationFreezeTime;
                weapon.StartShooting();
            }
            else
            {
                Debug.LogAssertion("Enemy has no attached Weapon but is ranged");
            }
        }
        else
        {
            animator.SetTrigger("Attack");
        }
    }

    public void EndAttack()
    {
        if (weapon != null)
        {
            weapon.EndShooting();
        }
    }

    public void Dash(float _dashSpeed = 0, float _dashDuration = 0)
    {
        dashSpeed = _dashSpeed == 0 ? dashSpeed: _dashSpeed;
        dashDuration = _dashDuration == 0 ? dashDuration : _dashDuration;
        dashDirection = transform.forward;
        if (!isDashing)
        {
            isDashing = true;
            Timer timer = gameObject.AddComponent<Timer>();
            timer.SetTimer(dashDuration, StopDash);
            Destroy(timer, dashDuration+(dashDuration/10));
        }
    }

    private void StopDash()
    {
        isDashing = false;
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
}