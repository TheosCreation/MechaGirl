using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class LobProjectile : Projectile
{
    // Start is called before the first frame update
    [SerializeField] protected float speed = 20.0f; // Speed of the projectile
    [SerializeField] protected bool destroyOnHit = true; // Does obj destroy on hit
    public UnityEvent onCollision;

    public override void Initialize(Vector3 startPosition, Vector3 direction, bool fromPlayer)
    {
        base.Initialize(startPosition, direction, fromPlayer);
        rb.useGravity = true;
        if (!fromPlayer)
        {
            //remove the Enemy layer from the hitMask
            RemoveEnemyFromHitMask();
        }
        // Apply force to move the projectile
        rb.AddForce(direction * speed, ForceMode.VelocityChange);

        // Destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit an object on the collisionMask
        if (other.gameObject.layer == ownerLayer) {  return; };
        if ((hitMask.value & (1 << other.gameObject.layer)) == 0) return;
        
        // Deal damage if the object is damageable
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage);
        }
        onCollision?.Invoke();
        // Destroy the projectile on collision
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        onCollision?.Invoke();
        // Destroy the projectile on collision
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
