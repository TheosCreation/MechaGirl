using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float health = 30.0f;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.gameObject.GetComponent<PlayerController>();
        if (controller != null) {
            controller.Heal(health);
            Destroy(gameObject);
        }
    }
}
