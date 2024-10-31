using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Flying enemy")]
    public float raycastDistance = 2.0f;
    public float moveSpeed = 5f;
    public int numberOfRays = 32;
    public float randomMovementInterval = 2.0f; // Time interval to change random direction
    [Range(0.1f, 5)]
    public float flatteningFactor = 2.0f; // Cntrol flattening
    private Vector3 randomDirection;
    private float timeSinceLastRandomDirectionChange;
    private Transform playerTransform;
    [Range(0, 2)]
    public float biasTowardsPlayer = 0.7f; 
    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        health = maxHealth;

        StateMachine = new EnemyStateMachineBuilder()
            .AddState(new LookingState())
            .AddState(new FlyingWonderState())
            .AddState(new FlyingAttackingState())
            .Build();

        SetDefaultState();
        delayTimer = gameObject.AddComponent<Timer>();
        launchTimer = gameObject.AddComponent<Timer>();
        weapon = GetComponentInChildren<Weapon>();
        rb.useGravity = false; // Disable gravity for flying
        rb.isKinematic = false; // Ensure Rigidbody is not kinematic for applying forces
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure the player has the tag 'Player'.");
        }

        ChangeRandomDirection();
    }
    protected new void Update()
    {
        StateMachine.Update(this);
        currentState = StateMachine.GetCurrentState();
        PerformRaycastMovement();
        UpdateRandomMovement();
    }

    private void PerformRaycastMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        bool playerDetected = false;

        float goldenAngle = Mathf.PI * (3 - Mathf.Sqrt(5)); // Approximate 2.39996323

        for (int i = 0; i < numberOfRays; i++)
        {
            // Calculate uniformly distributed points on a sphere using the Fibonacci sphere algorithm
            float t = (float)i / numberOfRays;
            float inclination = Mathf.Acos(1 - 2 * t); // Inclination angle (theta) from 0 to π
            float azimuth = goldenAngle * i; // Azimuthal angle (phi)

            // Convert spherical coordinates to Cartesian coordinates
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            Vector3 direction = new Vector3(x, y, z);

            // Apply flattening to the vertical component
            direction.y /= flatteningFactor;

            // Normalize the direction vector after flattening
            direction.Normalize();

            // Perform the raycast in the calculated direction
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, raycastDistance))
            {
                // Draw the ray in red if it hits something
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                if (hit.collider.CompareTag("Player"))
                {
                    playerDetected = true;
                    moveDirection += direction;
                    break;
                }
                else if (hit.collider.CompareTag("Untagged"))
                {
                    // Calculate avoidance force based on distance
                    float distance = hit.distance;
                    float avoidanceMultiplier = distance != 0 ? 1f / distance : 100f; // Avoid division by zero
                    moveDirection -= direction * avoidanceMultiplier;
                }
            }
            else
            {
                // Draw the ray in green if it doesn't hit anything
                Debug.DrawRay(transform.position, direction * raycastDistance, Color.green);
            }
        }

        if (playerDetected)
        {
            // Move towards the player
            rb.velocity = moveDirection.normalized * moveSpeed;
        }
        else
        {
            // Blend random movement with avoidance
            moveDirection += randomDirection;
            rb.velocity = moveDirection.normalized * moveSpeed;
        }
    }


    private void UpdateRandomMovement()
    {
        timeSinceLastRandomDirectionChange += Time.deltaTime;
        if (timeSinceLastRandomDirectionChange >= randomMovementInterval)
        {
            ChangeRandomDirection();
            timeSinceLastRandomDirectionChange = 0f;
        }
    }

    private void ChangeRandomDirection()
    {
        // Generate a random variation
        Vector3 randomVariation = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f) / flatteningFactor, // Flatten vertically
            Random.Range(-1f, 1f)
        ).normalized;

        if (playerTransform != null)
        {
            // Direction towards the player
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

            // Define how strongly the enemy is biased towards the player
            float biasTowardsPlayer = 0.7f; // Value between 0 (no bias) and 1 (full bias)

            // Blend the random variation with the direction to the player
            randomDirection = Vector3.Lerp(randomVariation, directionToPlayer, biasTowardsPlayer).normalized;
        }
        else
        {
            // If player is not found, use random variation
            randomDirection = randomVariation;
        }
    }

}
