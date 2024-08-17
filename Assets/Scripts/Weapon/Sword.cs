using UnityEngine;

public class Sword : Weapon
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float meleeDamage = 20.0f;
    [SerializeField] private float dashSpeed = 10.0f;
    [SerializeField] private float dashDuration = 1.0f;
    [SerializeField] private LayerMask targetLayer;
    
    
    public override void Shoot()
    {
       base.Shoot();
        if(transform.parent == null) { return; }

        if (playerController != null)
        {
            playerController.playerMovement.Dash(transform.parent.parent.forward, dashSpeed, dashDuration);
        }
        else
        {
            transform.parent.GetComponent<Enemy>().Dash(dashSpeed, dashDuration);
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Draw a wireframe sphere to represent the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
