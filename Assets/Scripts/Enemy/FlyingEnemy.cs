using System.Collections;
using UnityEngine;
public class FlyingEnemy : Enemy
{
    [Header("Flying Settings")]
    public float verticalMovementRange = 2.0f;
    public float verticalMovementSpeed = 1.0f;
    public float minimumHeightAboveGround = 1.0f;
    public GameObject[] children;
    private float originalY;
    public float movementInterval = 2.0f; // New variable for movement interval

    protected new void Start()
    {
        base.Start(); // Call the base class Start method
        originalY = transform.position.y;
        StartCoroutine(RandomVerticalMovement());
    }

    private IEnumerator RandomVerticalMovement()
    {
        while (true)
        {
            float groundLevel = GetGroundLevel();
            float minY = groundLevel + minimumHeightAboveGround;
            float targetY = Mathf.Max(minY, originalY + Random.Range(-verticalMovementRange, verticalMovementRange));

            float totalDuration = verticalMovementSpeed + movementInterval;
            float elapsedTime = 0f;

            while (elapsedTime < totalDuration)
            {
                float t = elapsedTime / totalDuration;
                foreach (var child in children)
                {
                    float newY = Mathf.Lerp(child.transform.position.y, targetY, t);
                    child.transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(movementInterval); // Wait before changing direction again
        }
    }

    private float GetGroundLevel()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point.y;
        }
        return transform.position.y; // Default to current position if no ground is detected
    }
}
