using Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanProjectile : Projectile 
{


    [Tab("Setup")]
    [SerializeField] protected float headShotMultiplier = 1.5f;
    [SerializeField] protected LayerMask hitMask;
    [SerializeField] protected float hitParticlesLifetime = 1.0f;
    [SerializeField] protected float particleOffset = 0.1f;
    [SerializeField] protected GameObject hitWallPrefab;
    [SerializeField] protected GameObject hitEnemyPrefab;
    [SerializeField] protected GameObject gunTrailPrefab;
    public override void Initialize(float damage, Vector3 direction, PlayerController pc)
    {
        playerController = pc;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
        {
            targetPoint = hit.point;
            HandleHit(hit);
       
        }
        else
        {
            targetPoint = transform.position + transform.forward * 1000f;
        }

        // Draw a debug ray to visualize the hitscan raycast
        Debug.DrawRay(transform.position, ray.direction, Color.red, 1.0f);

        if (gunTrailPrefab != null)
        {
            GameObject gunTrailObject = Instantiate(gunTrailPrefab, transform.position, Quaternion.LookRotation(targetPoint - transform.position));
            TrailMovement trail = gunTrailObject.GetComponent<TrailMovement>();
            trail.hitpoint = targetPoint;
            trail.hitnormal = hit.normal;
        }
    }
    private void HandleHit(RaycastHit hit)
    {
    
        GameObject hitParticles;
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            damageable = hit.collider.GetComponent<IDamageable>();
            print(damageable!=null);
        }
        
        if (damageable != null)
        {
            if (playerController)
            {
                UiManager.Instance.FlashHitMarker();
                print("shot from player hit");
            }
            else
            {
                print("shot from enemy hit");
            }
            if (hit.collider.gameObject.CompareTag("Body"))
            {
                damageable.Damage(damage);
          
            }
            else
            {
                damageable.Damage(damage * headShotMultiplier);
            }

            if (hitEnemyPrefab != null)
            {
                Vector3 spawnPosition = hit.point + hit.normal * particleOffset;
                hitParticles = Instantiate(hitEnemyPrefab, spawnPosition, Quaternion.LookRotation(-hit.normal));
                Destroy(hitParticles.gameObject, hitParticlesLifetime);
            }
        }
        else
        {
            if (hitWallPrefab != null)
            {
                Vector3 spawnPosition = hit.point + hit.normal * particleOffset;
                hitParticles = Instantiate(hitWallPrefab, spawnPosition, Quaternion.LookRotation(-hit.normal));
                Destroy(hitParticles.gameObject, hitParticlesLifetime);
            }
        }
        Destroy(gameObject);
    }
}
