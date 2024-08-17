using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SwordsSwing : PhysicalProjectile
{
    // Start is called before the first frame update
    private GameObject player;
    PlayerMovement playerMove;
    public override void Initialize(Vector3 direction, bool fromPlayer)
    {
        // Apply force to move the projectile
        player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMovement>();
        rb.AddForce(direction * speed, ForceMode.VelocityChange);

        // Destroy the projectile after its lifetime
       
    }
    private void Update()
    {
        transform.position = player.transform.position;
        if (!playerMove.isDashing)
        {
            Destroy(gameObject);
        }

    }

}
