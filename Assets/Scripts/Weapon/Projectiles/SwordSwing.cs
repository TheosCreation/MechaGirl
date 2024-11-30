using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SwordsSwing : PhysicalProjectile
{
    // Start is called before the first frame update
    public GameObject player;
    public PlayerMovement playerMove;
    public Enemy enemy;
    public override void Initialize(Vector3 startPosition, Vector3 direction, WeaponUser weaponUser)
    {
        base.Initialize(startPosition, direction, weaponUser);

        playerMove = owner.GetComponent<PlayerMovement>();
        if (playerMove == null)
        {
            enemy = owner.GetComponent<Enemy>();
        }
        else
        {
            transform.localScale *= 3;
        }
        transform.rotation = new Quaternion(0,0,0,0);

    }
    private void Update()
    {
        if (owner == null) { 
            Destroy(gameObject); 
            return;
        }
        transform.position = owner.transform.position;
        if (playerMove != null)
        {
            if (!playerMove.isDashing)
            {
                Destroy(gameObject);                
            }
            return;
        }
        if(enemy != null)
        {
            if (!enemy.isDashing)
            {
                Destroy(gameObject);              
            }
            return;
        }
       

    }

}
