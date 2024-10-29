using System.Collections;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Flying Settings")]
    public float verticalMovementRange = 2.0f;
    public float verticalMovementSpeed = 1.0f;
    public GameObject[] children;
    private float originalY;

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
            float targetY = originalY + Random.Range(-verticalMovementRange, verticalMovementRange);
            float elapsedTime = 0f;

            while (elapsedTime < verticalMovementSpeed)
            {
                float newY = Mathf.Lerp(transform.position.y, targetY, (elapsedTime / verticalMovementSpeed));
                foreach (var child in children)
                {
                    child.transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                }
               
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1f, 3f)); // Wait before changing direction again
        }
    }
}
