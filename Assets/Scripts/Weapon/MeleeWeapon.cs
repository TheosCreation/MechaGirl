using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Attack Settings")]
    [SerializeField] protected float meleeRange = 1.5f;
    [SerializeField] protected float meleeDamage = 20.0f;
    [SerializeField] protected LayerMask targetLayer;

    protected override void Attach()
    {
        base.Attach();
        spriteBillboard.enabled = false;
    }

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