//using UnityEngine;
//
//public class SpriteBillboard : MonoBehaviour
//{
//    private void Update()
//    {
//        transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
//    }
//}

using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0; // Keep only the horizontal direction

        // Determine the rotation needed to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = targetRotation;
    }
}
