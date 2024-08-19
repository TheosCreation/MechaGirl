using UnityEngine;
using UnityEngine.Events;

public class TriggerFinishLevel : MonoBehaviour
{
    private bool active = true;

    // This method is called when another collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if (active)
        {
            LevelManager.Instance.CompleteLevel();
            active = false;
        }
    }
}