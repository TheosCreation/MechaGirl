using UnityEngine;

public abstract class WeaponUser : MonoBehaviour
{
    public abstract Transform GetFirePoint();

    public abstract Vector3 GetForwardDirection();

    public abstract LayerMask GetHitMask();
    public abstract Projectile GetProjectilePrefab(Weapon weapon);

    // An event function to call other functions
    public virtual void OnWeaponFire(Weapon weaponFired) { }

    // An event function to call other functions
    public virtual void OnPickUp() { }
    
    // An event function to call other functions
    public virtual void OnHit() { }
}
