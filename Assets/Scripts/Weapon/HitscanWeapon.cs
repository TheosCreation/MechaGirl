using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HitscanWeapon : Weapon
{
    private bool isPreparingToShoot = false;

    public override void Shoot()
    {
        if (playerController == null)
        {
            if (!isPreparingToShoot)
            {
                StartCoroutine(PrepareToShoot());
            }
        }
        else
        {
            base.Shoot();
        }
    }

    private IEnumerator PrepareToShoot()
    {
        isPreparingToShoot = true;

        // Set the shot direction
        shotDirection = transform.forward;

        // Wait for 1 second
        yield return new WaitForSeconds(1.0f);

        // Call the base Shoot method
        base.Shoot();

        isPreparingToShoot = false;
    }
}
