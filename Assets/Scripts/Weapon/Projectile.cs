using Runtime;
using UnityEngine;


public class Projectile : MonoBehaviour
{

    [Tab("Base")]
    [SerializeField] protected float lifetime = 5.0f; // Lifetime of the projectile
    [SerializeField] protected LayerMask collisionMask; // Layers to check for collision
    [SerializeField] protected float damage = 10.0f;

    protected PlayerController playerController;

    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Initialize(float damage, Transform target, PlayerController pc) { }

   
}
