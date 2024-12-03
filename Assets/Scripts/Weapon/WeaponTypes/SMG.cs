using System.Collections;
using UnityEngine;

public class SMG : HitscanWeapon
{

    public override void UseAbility()
    {
        base.UseAbility();
        
        PlayRandomFiringSound();
        Ammo -= 10;
        int projectileCount = 50;
        float angleStep = Mathf.PI * 2 / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angleX = Random.Range(-Mathf.PI, Mathf.PI);
            float angleY = Random.Range(0, Mathf.PI * 2);

            Vector3 randomDirection = new Vector3(
                Mathf.Cos(angleX) * Mathf.Sin(angleY),
                Mathf.Sin(angleX),
                Mathf.Cos(angleX) * Mathf.Cos(angleY)
            );

            Projectile projectile = Instantiate(weaponUser.GetProjectilePrefab(this), transform.position, Quaternion.identity);
            projectile.owner = gameObject;
            projectile.ownerLayer = gameObject.layer;
            projectile.hitMask = weaponUser.GetHitMask();

            projectile.Initialize(transform.position, randomDirection, weaponUser);
        }

    }

    private IEnumerator ShootInAllDirections()
    {
        yield return new WaitForSeconds(0.1f);
        PlayRandomFiringSound();
        for (int i = 0; i < 50; i++)
        {
            // Generate a random direction
            Vector3 randomDirection = Random.insideUnitSphere.normalized;


         
            Projectile projectile = Instantiate(weaponUser.GetProjectilePrefab(this), transform.position, Quaternion.identity);
       
            projectile.owner = gameObject;
            projectile.ownerLayer = gameObject.layer;
            projectile.hitMask = weaponUser.GetHitMask();

            projectile.Initialize(transform.position, randomDirection, weaponUser);
        }
    }
    void OnCollisionEnter(Collision collision)
    {

    }
}