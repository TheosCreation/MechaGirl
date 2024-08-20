using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    PlayerController player;
    Enemy enemy;

    bool isPlayer = true;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();

        if(player == null)
        {
            enemy = GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                isPlayer = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            if (isPlayer)
            {
                Weapon weapon = other.gameObject.GetComponent<Weapon>();
                weapon.PickUp(player.weaponHolder, player);
            }
            else
            {
                //is enemy and pick up similar to player honestly doesnt need the isPlayer bool if done right
            }    
        }
    }
}