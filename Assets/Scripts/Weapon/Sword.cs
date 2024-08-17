using UnityEngine;

public class Sword : Weapon
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float meleeDamage = 20.0f;
    [SerializeField] private float dashLength = 10.0f;
    [SerializeField] private LayerMask targetLayer;

    // Override the Awake method to call the base class's Awake method
    public override void Shoot()
    {
       base.Shoot();
        if(transform.parent == null) { return; }
        playerController.playerMovement.Dash(transform.parent.parent.forward, 12, 0.1f);
    }
    private void OnDrawGizmosSelected()
    {
        // Draw a wireframe sphere to represent the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
