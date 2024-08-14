using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TriggerDoor : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private bool locked = false;
    private NavMeshSurface _navMeshSurface;

    void Start()
    {
        _animator = GetComponent<Animator>();

        _navMeshSurface = GetComponentInParent<NavMeshSurface>();
        if (_navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface component not found in parent hierarchy.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !locked)
        {
            _animator.SetBool("Open", true);
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
    }

    public void Unlock()
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
}