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
    [SerializeField] private bool rotateY = false;

    private void Update()
    {
        //Transform target = LevelManager.Instance.playerSpawn.playerSpawned.transform;
        //Vector3 directionToTarget = target.position - transform.position;
        //if(!rotateY) directionToTarget.y = 0; // Keep only the horizontal direction

        // Determine the rotation needed to look at the target
        Vector3 targetRotation = Camera.main.transform.rotation.eulerAngles;
        if (!rotateY) targetRotation.y = 0;

        transform.rotation = Quaternion.Euler(targetRotation);
    }
}
