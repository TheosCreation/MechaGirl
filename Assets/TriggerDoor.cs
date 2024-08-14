using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private bool locked = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _animator.SetTrigger("Open");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _animator.SetTrigger("Close");
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
}
