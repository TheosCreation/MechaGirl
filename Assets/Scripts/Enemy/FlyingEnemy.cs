using System.Collections;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float raycastDistance = 2.0f;
    public float moveSpeed = 5f;
    public int numberOfRays = 32;
    public float randomMovementInterval = 2.0f; // Time interval to change random direction
    public float flatteningFactor = 2.0f; // Cntrol flattening
    private Vector3 randomDirection;
    private float timeSinceLastRandomDirectionChange;

    protected new void Start()
    {
        base.Start();
        agent.enabled = false; // Disable NavMeshAgent
        rb.useGravity = false; // Disable gravity for flying
        rb.isKinematic = false; // Ensure Rigidbody is not kinematic for applying forces
        ChangeRandomDirection();
    }

    protected new void Update()
    {
        PerformRaycastMovement();
        UpdateRandomMovement();
    }

    private void PerformRaycastMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        bool playerDetected = false;

        // Cast sphere ray
        for (int i = 0; i < numberOfRays; i++)
        {
            float theta = Mathf.Acos(2 * (i / (float)numberOfRays) - 1);
            float phi = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

            // Adjust the direction to flatten the sphere
            Vector3 direction = new Vector3(
                Mathf.Sin(theta) * Mathf.Cos(phi),
                Mathf.Sin(theta) * Mathf.Sin(phi) / flatteningFactor, // Flatten vertically so to further range
                Mathf.Cos(theta)
            );

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
        // Generate a new random direction
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f) / flatteningFactor, // Flatten vertically
            Random.Range(-1f, 1f)
        ).normalized;
    }
}
