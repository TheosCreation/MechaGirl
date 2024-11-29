using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponUser : MonoBehaviour
{
    public abstract Transform GetFirePoint();

    public abstract Vector3 GetForwardDirection();

    public abstract LayerMask GetHitMask();
    public abstract Projectile GetProjectilePrefab(Weapon weapon);

    public virtual void OnWeaponFire(Projectile newProjectile, bool fromPlayer = false)
    {

        Projectile projectile = Instantiate(newProjectile, GetFirePoint().position, Quaternion.identity);
       // projectile.hitMask = GetHitMask();
        projectile.owner = gameObject;
        projectile.ownerLayer = gameObject.layer;

        projectile.Initialize(GetFirePoint().position, GetForwardDirection(), fromPlayer);
    }

    public virtual void OnPickUp()
    {

    }
}
