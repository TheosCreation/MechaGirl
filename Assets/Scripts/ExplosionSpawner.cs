using UnityEngine;

public class ExplosionSpawner : MonoBehaviour
{
    [SerializeField] private Explosion explosionPrefab;
    [SerializeField] private float explosionDamage = 5.0f;

    public void SpawnExplosion()
    {
      
        Explosion explosionObjectSpawned = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosionObjectSpawned.damage = explosionDamage;

        //Rework needed
        Projectile spawner = GetComponent<Projectile>();
        
        if(spawner != null )
        {
            if (spawner.ownerTag == "Player")
            {
                explosionObjectSpawned.fromEnemy = false;
            }
        }
    }
}