using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDoor : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private bool locked = false;
    private NavMeshSurface _navMeshSurface;
    [SerializeField] private float minAnimSpeed = 1.0f;
    [SerializeField] private float speedMultiplier = 0.1f;

    [SerializeField] private GameObject overlay;

    public UnityEvent onTriggerEnter;
    private bool triggerEventActive = true;

    void Start()
    {
        _animator = GetComponent<Animator>();

        _navMeshSurface = GetComponentInParent<NavMeshSurface>();
        if (_navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface component not found in parent hierarchy.");
        }

        UpdateOverlay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !locked)
        {
            _animator.SetBool("Open", true); 
            if (triggerEventActive)
            {
                onTriggerEnter?.Invoke();
                triggerEventActive = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !locked)
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                AdjustDoorAnimationByPlayerSpeed(playerRb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetBool("Open", false);
        }
    }

    public void Lock()
    {
        locked = true;
        _animator.SetBool("Open", false);
        UpdateOverlay();
    }

    public void Unlock()
    {
        locked = false;
        UpdateOverlay();
    }

    public void UpdateNavMesh()
    {
        if (_navMeshSurface != null)
        {
            _navMeshSurface.BuildNavMesh();
        }
    }

    private void UpdateOverlay()
    {
        if (overlay != null)
        {
            overlay.SetActive(locked);
        }
    }

    public void AdjustDoorAnimationByPlayerSpeed(Rigidbody rb)
    {
        // Get the horizontal velocity magnitude
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        // Scale the animator speed based on horizontal speed
        _animator.speed = minAnimSpeed + horizontalSpeed * speedMultiplier;
    }

    public void Reset()
    {
        Unlock();
    }
}