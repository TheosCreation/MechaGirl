using UnityEngine;
using UnityEngine.Events;

public class TriggerCheckPoint : MonoBehaviour
{
    public UnityEvent OnPlayerRespawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        LevelManager.Instance.SetCheckPoint(transform);
        LevelManager.Instance.OnPlayerRespawn = OnPlayerRespawn;
        Destroy(gameObject);
    }
}