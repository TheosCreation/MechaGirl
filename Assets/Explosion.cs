using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float despawnTimer = 2.0f;
    public float damage = 1.0f;

    private void Start()
    {
        Destroy(gameObject, despawnTimer);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null )
        {
            damageable.Damage( damage );
        }
    }
}