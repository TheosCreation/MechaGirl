using UnityEngine;


public class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed = 20.0f; // Speed of the projectile
    [SerializeField] protected float lifetime = 5.0f; // Lifetime of the projectile
    [SerializeField] protected LayerMask collisionMask; // Layers to check for collision

    protected Rigidbody rb;
    protected float damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponentInParent<PlayerController>();
    }

    public virtual void Initialize(float damage, Transform target) { }

   
}
