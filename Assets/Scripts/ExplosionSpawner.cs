     using UnityEngine;

public class ExplosionSpawner : MonoBehaviour
{
    [SerializeField] private Explosion explosionPrefab;
    [SerializeField] private float explosionDamage = 5.0f;

    public void SpawnExplosion()
    {
      
        Explosion explosionObjectSpawned = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosionObjectSpawned.damage = explosionDamage;
        Projectile spawner = GetComponent<Projectile>();
        
        if (spawner.ownerTag == "Player")
        {
            explosionObjectSpawned.enemy = false;
        }
        
    }
}