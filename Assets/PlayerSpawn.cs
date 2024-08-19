using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject player;

    void Awake()
    {
        Instantiate(player, transform.position, transform.rotation);
    }
}
