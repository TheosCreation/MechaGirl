using UnityEngine;

public class WeaponSpawner : IResetable
{
    private SpriteRenderer sr;
    public bool isActive = true;
    public WeaponSpawn weaponToSpawn;
    private Weapon weaponSpawned;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;

        SpawnWeapon();
    }

    void SpawnWeapon()
    {
        if (!isActive) return;

        weaponSpawned = Instantiate(weaponToSpawn.weaponPrefab, transform);
        weaponSpawned.transform.localPosition = Vector3.zero;
        weaponSpawned.canPickup = true;
        weaponSpawned.bc.enabled = true;
        weaponSpawned.rb.isKinematic = false;
        weaponSpawned.startingAmmo = weaponToSpawn.startingAmmo;
    }

    public override void Reset()
    {
        if (weaponSpawned != null)
        {
            Destroy(weaponSpawned);
        }

        SpawnWeapon();
    }

    public void DeActivate()
    {
        isActive = false;
    }
}
