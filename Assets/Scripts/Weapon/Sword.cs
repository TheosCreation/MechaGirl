using UnityEngine;

public class Sword : Weapon
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float meleeDamage = 20.0f;
    [SerializeField] private float dashLength = 10.0f;
    [SerializeField] private float dashDuration = 1.0f;
    [SerializeField] private LayerMask targetLayer;
    
    
    public override void Shoot()
    {
       base.Shoot();
        if(transform.parent == null) { return; }

        playerController.playerMovement.Dash(transform.parent.parent.forward, dashLength, dashDuration);
        
    }
    private void OnDrawGizmosSelected()
    {
        // Draw a wireframe sphere to represent the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
