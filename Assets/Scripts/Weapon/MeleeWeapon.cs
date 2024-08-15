using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float meleeDamage = 20.0f;
    [SerializeField] private LayerMask targetLayer;

    public override void Shoot()
    {
        // Perform a slicing attack and deal damage to enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, meleeRange, targetLayer);

        foreach (Collider enemy in hitEnemies)
        {
            IDamageable enemyHealth = enemy.GetComponentInParent<IDamageable>();
            if (enemyHealth != null)
            {
                enemyHealth.Damage(meleeDamage);
            }
        }

        animator.SetTrigger("Shoot");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wireframe sphere to represent the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}