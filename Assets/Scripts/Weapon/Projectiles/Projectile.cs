using Runtime;
using UnityEngine;


public class Projectile : MonoBehaviour
{

    [Tab("Base")]
    [SerializeField] protected float lifetime = 5.0f; // Lifetime of the projectile
    [SerializeField] protected LayerMask hitMask;
    [SerializeField] protected float damage = 10.0f;

    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public virtual void Initialize(Vector3 direction, bool fromPlayer) { }
}
