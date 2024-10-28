using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PhysicalProjectile : Projectile
{
    // Start is called before the first frame update
    [SerializeField] protected float speed = 20.0f; // Speed of the projectile
    [SerializeField] protected bool destroyOnHit = true; // Does obj destroy on hit

    public override void Initialize(Vector3 direction, bool fromPlayer)
    {
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

        // Destroy the projectile on collision
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
