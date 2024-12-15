using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PhysicalProjectile : Projectile
{
    // Start is called before the first frame update
    [SerializeField] protected float speed = 20.0f; // Speed of the projectile
    [SerializeField] protected float headShotMultiplier = 1.5f; // Speed of the projectile
    [SerializeField] protected bool destroyOnHit = true; // Does obj destroy on hit
    public UnityEvent onCollision;

    public override void Initialize(Vector3 startPosition, Vector3 direction, WeaponUser weaponUser)
    {
        base.Initialize(startPosition, direction, weaponUser);
        // Apply force to move the projectile
        rb.AddForce(direction * speed, ForceMode.VelocityChange);

        // Destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit an object on the collisionMask
        if (other.gameObject.layer == ownerLayer) { return; };
        if ((hitMask.value & (1 << other.gameObject.layer)) == 0) return;

        ProcessCollision(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ProcessCollision(collision.collider);
    }

    protected void ProcessCollision(Collider other)
    {
        // Calculate the hit point and normal
        Vector3 hitPoint = Physics.ClosestPoint(transform.position, other, other.transform.position, other.transform.rotation);
        Vector3 hitNormal = (transform.position - hitPoint).normalized;

        if (hitNormal == Vector3.zero)
        {
            hitNormal = Vector3.up; // Fallback normal
        }

        // Deal damage if the object is damageable
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            m_weaponUser.OnHit();

            if (other.gameObject.CompareTag("Head"))
            {
                damageable.Damage(damage * headShotMultiplier);
                HitDamageable(hitPoint, hitNormal, GameManager.Instance.prefabs.hitEnemyPrefab, GameManager.Instance.prefabs.enemyWeakspotHitSound);
            }
            else
            {
                damageable.Damage(damage);
                HitDamageable(hitPoint, hitNormal, GameManager.Instance.prefabs.hitEnemyPrefab, GameManager.Instance.prefabs.enemyWeakspotHitSound);
            }
        }
        else
        {
            // hit a wall
            HitWall(hitPoint, hitNormal);
        }

        onCollision?.Invoke();
        // Destroy the projectile on collision
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
