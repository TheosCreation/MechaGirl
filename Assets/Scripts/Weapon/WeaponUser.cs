using UnityEngine;

public abstract class WeaponUser : MonoBehaviour
{
    public virtual Vector3 GetFirePoint()
    {
        return Vector3.zero;
    }

    public abstract Vector3 GetForwardDirection();

    public abstract LayerMask GetHitMask();
    public abstract Projectile GetProjectilePrefab(Weapon weapon);
    public abstract void DashForward(float speed, float dashDuration);

    // An event function to call other functions
    public virtual void OnWeaponFire(Weapon weaponFired) { }

    // An event function to call other functions
    public virtual void OnPickUp() { }
    
    // An event function to call other functions
    public virtual void OnHit() { }

    public virtual void OnAmmoChange(int currentAmmo) { }
}
