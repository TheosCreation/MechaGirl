using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject player;

    void Awake()
    {
        CapsuleCollider playerCollider = player.GetComponent<CapsuleCollider>();
        Vector3 spawnOffset = new Vector3(0, playerCollider.height / 2, 0);
        Instantiate(player, transform.position + spawnOffset, transform.rotation);
    }
}
