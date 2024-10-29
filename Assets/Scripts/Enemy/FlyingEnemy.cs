using System.Collections;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Movement Settings")]
    public float raycastDistance = 2.0f;
    public float moveSpeed = 5f;
    public int numberOfRays = 64; 
    public float flatteningFactor = 2.0f;
    public float randomMovementIntensity = 2.0f; // Control random movement intensity
    public float chaseIntensity = 5.0f; // How hard it chases the player

    [Header("Steering Settings")]
    public float maxSpeed = 5f;
    public float maxForce = 10f;
    public float mass = 1f; // Mass of the enemy for acceleration calculations
    private Vector3 velocity;
    private Vector3 steering;

    [Header("Random Movement Settings")]
    public float randomMovementInterval = 1.0f; // Reduced from 2.0f

    [Header("Obstacle Avoidance Settings")]
    public float obstacleAvoidanceIntensity = 1.0f; // Control obstacle avoidance intensity
    public float obstacleAvoidanceThreshold = 1.0f; // Threshold distance for obstacle avoidance

    private Vector3 randomDirection;
    private float timeSinceLastRandomDirectionChange;
    private Vector3 wanderTarget;

    // Bounds for the movement area (adjust as needed)
    [Header("Movement Bounds")]
    public float minX = -10f, maxX = 10f;
    public float minY = 0f, maxY = 5f;
    public float minZ = -10f, maxZ = 10f;
    protected new void Start()
    {
        base.Start();
        velocity = Vector3.zero;
        steering = Vector3.zero; 
        agent.enabled = false;
        rb.useGravity = false;
        wanderTarget = Random.onUnitSphere * 2.0f;
        rb.isKinematic = false;
        ChangeRandomDirection();
    }

    protected new void Update()
    {
        PerformRaycastMovement();
    }

    private void PerformRaycastMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        Vector3 desiredVelocity = Vector3.zero;
        bool playerDetected = false;


        for (int i = 0; i < numberOfRays; i++)
        {
            float theta = Random.Range(0f, Mathf.PI * 2f); // Azimuthal angle
            float phi = Mathf.Acos(2f * Random.Range(0f, 1f) - 1f); // Polar angle

            Vector3 direction = new Vector3(
                Mathf.Sin(phi) * Mathf.Cos(theta),
                Mathf.Sin(phi) * Mathf.Sin(theta),
                Mathf.Cos(phi)
            );


            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, raycastDistance))
            {
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                if (hit.collider.CompareTag("Player"))
                {
                    playerDetected = true;
                    moveDirection += direction * chaseIntensity;
                    break; // Found the player, no need to check other rays
                }
                else if (hit.collider.CompareTag("Untagged"))
                {
                    float distance = hit.distance;
                    if (distance < obstacleAvoidanceThreshold)
                    {
                        float avoidanceMultiplier = (obstacleAvoidanceThreshold - distance) / obstacleAvoidanceThreshold;
                        Vector3 avoidanceDirection = hit.normal; // Use the normal to push away from the obstacle
                        moveDirection += avoidanceDirection * avoidanceMultiplier * obstacleAvoidanceIntensity;
                        // Visualize the avoidance direction
                        Debug.DrawRay(hit.point, avoidanceDirection, Color.blue);
                    }
                }
            }
            else
            {
                Debug.DrawRay(transform.position, direction * raycastDistance, Color.green);
            }
        }
        if (playerDetected)
        {
            // Move towards the player
            desiredVelocity += moveDirection.normalized * maxSpeed;
        }
        else
        {
            // Wander around
            UpdateRandomMovement();
            Wander(ref desiredVelocity);
            KeepWithinBounds(ref desiredVelocity);
        }

        // Obstacle avoidance steering
        ObstacleAvoidance(ref desiredVelocity);

        // Calculate steering force
        steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering /= mass; // Apply mass

        // Update velocity and position
        velocity = Vector3.ClampMagnitude(velocity + steering * Time.deltaTime, maxSpeed);
        rb.velocity = velocity;
    }
    private void ObstacleAvoidance(ref Vector3 desiredVelocity)
    {
        RaycastHit hit;
        // Cast rays in the direction of current velocity
        if (Physics.Raycast(transform.position, velocity.normalized, out hit, obstacleAvoidanceThreshold))
        {
            if (hit.collider.CompareTag("Untagged"))
            {
                Vector3 avoidanceDirection = Vector3.Reflect(velocity.normalized, hit.normal);
                avoidanceDirection.Normalize();
                desiredVelocity += avoidanceDirection * obstacleAvoidanceIntensity * (1f - hit.distance / obstacleAvoidanceThreshold);
                Debug.DrawRay(hit.point, avoidanceDirection * obstacleAvoidanceIntensity, Color.red);
            }
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
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f) / flatteningFactor,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    private void Wander(ref Vector3 desiredVelocity)
    {
        float wanderRadius = 2.0f;
        float wanderDistance = 5.0f;
        float wanderJitter = 1.0f;

        // Update the target position on the wander circle
        wanderTarget += new Vector3(
            Random.Range(-1f, 1f) * wanderJitter,
            Random.Range(-1f, 1f) * wanderJitter,
            Random.Range(-1f, 1f) * wanderJitter
        );

        // Normalize and set to wander radius
        wanderTarget = wanderTarget.normalized * wanderRadius;

        // Calculate target position in world space
        Vector3 targetLocal = wanderTarget + Vector3.forward * wanderDistance;
        Vector3 targetWorld = transform.TransformPoint(targetLocal);

        // Set desired velocity towards the target
        Vector3 wanderDirection = (targetWorld - transform.position).normalized * maxSpeed;
        desiredVelocity += wanderDirection * randomMovementIntensity;
    }

    private void KeepWithinBounds(ref Vector3 moveDirection)
    {
        Vector3 position = transform.position;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);

        Vector3 directionToCenter = (center - position).normalized;

        if (position.x < minX || position.x > maxX || position.y < minY || position.y > maxY || position.z < minZ || position.z > maxZ)
        {
            moveDirection += directionToCenter * obstacleAvoidanceIntensity;
        }
    }
}
