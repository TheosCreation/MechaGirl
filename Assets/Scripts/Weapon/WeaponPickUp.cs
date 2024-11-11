using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    PlayerController player;
    Enemy enemy;

    bool isPlayer = true;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();

        if (player == null)
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
                //If the player has picked up the weapon then we do ui and audio
                if(weapon.PickUp(player.weaponHolder, player, false))
                {
                    System.Type weaponType = weapon.GetType(); 
                    if (!UiManager.Instance.HasPickedUpWeaponType(weaponType))
                    {
                        UiManager.Instance.MarkWeaponTypeAsPickedUp(weaponType);
                        UiManager.Instance.ShowPickUpAnimation(weapon.iconSprite);
                    }
                }
            }
            else
            {
                // Handle enemy pickup logic here
            }
        }
    }
}