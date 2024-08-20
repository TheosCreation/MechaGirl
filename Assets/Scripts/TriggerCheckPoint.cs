using UnityEngine;

public class TriggerCheckPoint : MonoBehaviour
{
    private bool active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if(active)
        {
            LevelManager.Instance.SetCheckPoint(transform);
            active = false;
        }
    }
}