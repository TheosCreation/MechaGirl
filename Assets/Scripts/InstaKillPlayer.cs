using UnityEngine;

public class InstaKillPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.Damage(1000.0f);
        }
    }
}
