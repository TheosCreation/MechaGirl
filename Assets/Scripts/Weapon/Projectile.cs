using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20.0f; // Speed of the projectile
    [SerializeField] private float lifetime = 5.0f; // Lifetime of the projectile
    [SerializeField] private LayerMask collisionMask; // Layers to check for collision

    private Rigidbody rb;
    private float damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(float damage, Transform target)
    {
        this.damage = damage;

        // Calculate direction towards target
        Vector3 direction;
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward;
        }

        // Apply force to move the projectile
        rb.AddForce(direction * speed, ForceMode.VelocityChange);

        // Destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit an object on the collisionMask
        if ((collisionMask.value & (1 << other.gameObject.layer)) == 0) return;

        // Deal damage if the object is damageable
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage);
        }

        // Destroy the projectile on collision
        Destroy(gameObject);
    }
}
