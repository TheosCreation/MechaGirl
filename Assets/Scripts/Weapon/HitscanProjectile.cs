using Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanProjectile : Projectile 
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float headShotMultiplier = 1.5f;


    [Tab("Setup")]
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float hitParticlesLifetime = 1.0f;
    [SerializeField] private float particleOffset = 0.1f;
    [SerializeField] private GameObject hitWallPrefab;
    [SerializeField] private GameObject hitEnemyPrefab;
    [SerializeField] private GameObject gunTrailPrefab;
    public override void Initialize(float damage, Transform target)
    {
        Ray ray = new Ray(transform.position, transform.forward);
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
        Debug.DrawRay(transform.position, transform.forward * 1000f, Color.red, 1.0f);

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
        }

        if (damageable != null)
        {
            if (playerController)
            {
                UiManager.Instance.FlashHitMarker();
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
    }
}
