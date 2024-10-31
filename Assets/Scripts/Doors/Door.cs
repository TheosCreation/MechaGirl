using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : IResetable
{
    protected Animator _animator;
    protected NavMeshSurface _navMeshSurface;
    [SerializeField] protected float minAnimSpeed = 1.0f;
    [SerializeField] protected float speedMultiplier = 0.1f;
    [SerializeField] protected bool locked = false;

    private Timer lockTimer;

    protected virtual void Start()
    {
        _animator = GetComponent<Animator>();

        _navMeshSurface = GetComponentInParent<NavMeshSurface>();
        if (_navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface component not found in parent hierarchy.");
        }

        lockTimer = gameObject.AddComponent<Timer>();

    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Body")) && !locked)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                AdjustDoorAnimationBySpeed(rb.velocity);
            }
            else
            {
                NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    AdjustDoorAnimationBySpeed(agent.velocity);
                }
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Body"))
        {
            _animator.SetBool("Open", false);
        }
    }
    public virtual void Lock()
    {
        locked = true;
        _animator.SetBool("Open", false);
        lockTimer.SetTimer(0.5f, UpdateNavMesh);
    }

    public virtual void Unlock()
    {
        locked = false;
    }

    public void UpdateNavMesh()
    {
        if (_navMeshSurface != null)
        {
            _navMeshSurface.BuildNavMesh();
        }
    }


    public void AdjustDoorAnimationBySpeed(Vector3 velocity)
    {
        // Get the horizontal velocity magnitude
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        // Scale the animator speed based on horizontal speed
        _animator.speed = minAnimSpeed + horizontalSpeed * speedMultiplier;
    }

    public override void Reset()
    {
        Unlock();
    }
}