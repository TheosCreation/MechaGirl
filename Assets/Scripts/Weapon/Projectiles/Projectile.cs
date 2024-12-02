using Runtime;
using UnityEngine;


public class Projectile : MonoBehaviour
{

    [Tab("Base")]
    [SerializeField] protected float lifetime = 5.0f; // Lifetime of the projectile
    [SerializeField] public LayerMask hitMask;
    [SerializeField] protected float damage = 10.0f;
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
}
