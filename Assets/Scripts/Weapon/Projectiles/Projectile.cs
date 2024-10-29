using Runtime;
using UnityEngine;


public class Projectile : MonoBehaviour
{

    [Tab("Base")]
    [SerializeField] protected float lifetime = 5.0f; // Lifetime of the projectile
    [SerializeField] protected LayerMask hitMask;
    [SerializeField] protected float damage = 10.0f;
    public GameObject owner;
    public int ownerLayer = -1;
    protected Rigidbody rb;

    private void Awake()
    {   

        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);

    }

    public virtual void Initialize(Vector3 startPosition, Vector3 direction, bool fromPlayer)
    {
    }

    protected void RemoveEnemyFromHitMask()
    {
        int hitMaskValue = hitMask.value;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        hitMaskValue &= ~(1 << enemyLayer);
        hitMask = (LayerMask)hitMaskValue;
    }
}
