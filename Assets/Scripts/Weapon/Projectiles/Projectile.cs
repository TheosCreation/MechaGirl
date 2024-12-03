using Runtime;
using UnityEngine;


public class Projectile : MonoBehaviour
{

    [Tab("Base")]
    [SerializeField] protected float lifetime = 5.0f; // Lifetime of the projectile
    [SerializeField] protected LayerMask hitMask;
    [SerializeField] protected float damage = 10.0f;
    [SerializeField] protected float hitParticlesLifetime = 1.0f;
    [SerializeField] protected float particleOffset = 0.1f; // how much it is offset towards the weapon user

    public GameObject owner;
    public int ownerLayer = -1;
    public string ownerTag;
    protected Rigidbody rb;

    protected WeaponUser m_weaponUser;

    protected virtual void Awake()
    {   

        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);

    }

    public virtual void Initialize(Vector3 startPosition, Vector3 direction, WeaponUser weaponUser)
    {
        ownerTag = owner.tag;
        m_weaponUser = weaponUser;
        hitMask = weaponUser.GetHitMask();
        if (m_weaponUser is Enemy)
        {
            RemoveEnemyFromHitMask();
        }
    }

    protected void RemoveEnemyFromHitMask()
    {
        int hitMaskValue = hitMask.value;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        hitMaskValue &= ~(1 << enemyLayer);
        hitMask = (LayerMask)hitMaskValue;
    }

    protected void CreateHitSound(AudioClip clip, Vector3 spawnPosition)
    {
        GameObject soundMakerObject = new GameObject("SoundMaker");
        soundMakerObject.transform.position = spawnPosition;
        SoundMaker soundMaker = soundMakerObject.AddComponent<SoundMaker>();
        soundMaker.PlaySound(clip, 0.1f);
    }

    protected void HitDamageable(Vector3 hitPosition, Vector3 normal, GameObject particleToSpawn, AudioClip audioToPlay)
    {
        GameObject hitParticles = Instantiate(particleToSpawn, hitPosition, Quaternion.LookRotation(-normal));
        Destroy(hitParticles, hitParticlesLifetime);

        CreateHitSound(audioToPlay, hitPosition);
    }

    protected void HitWall(Vector3 hitPosition, Vector3 wallNormal)
    {
        GameObject hitParticles = Instantiate(GameManager.Instance.prefabs.hitWallPrefab, hitPosition, Quaternion.LookRotation(-wallNormal));
        Destroy(hitParticles, hitParticlesLifetime);

        CreateHitSound(GameManager.Instance.prefabs.wallHitSound, hitPosition);
    }
}
