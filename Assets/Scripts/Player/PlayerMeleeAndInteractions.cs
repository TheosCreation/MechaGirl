using UnityEngine;

public class PlayerMeleeAndInteractions : MonoBehaviour
{
    public bool canMelee = true;
    public Transform interactableTransform;
    [SerializeField] private float meleeDistance = 2.0f;
    [SerializeField] private float meleeDamage = 10.0f;
    [SerializeField] private float meleeCooldown = 1.5f;
    [SerializeField] private float throwForce = 2.0f;
    [SerializeField] private LayerMask hitMask;
    private PlayerController playerController;
    private Timer meleeTimer;
    private bool HasHitDamagable = false;
    private bool HasHitInteractble = false;
    public Keycard currentHeldKeycard;

    private void Awake()
    {
        InputManager.Instance.playerInput.InGame.Melee.started += _ctx => Melee();

        playerController = GetComponent<PlayerController>();
        meleeTimer = gameObject.AddComponent<Timer>();
    }

    private void Melee()
    {
        if (!canMelee) return;
        Debug.Log("Melee");

        RaycastHit[] hits = Physics.RaycastAll(transform.position, playerController.playerCamera.transform.forward, meleeDistance, hitMask);
        foreach (RaycastHit hit in hits)
        {
            HandleHit(hit.collider);
        }
        if(!HasHitInteractble)
        {
            if(currentHeldKeycard != null)
            {
                currentHeldKeycard.Throw(throwForce, playerController.playerCamera.transform.forward);
                currentHeldKeycard = null;
            }
        }

        HasHitDamagable = false;
        HasHitInteractble = false;
        canMelee = false;
        meleeTimer.SetTimer(meleeCooldown, () => canMelee = true);
    }

    private void HandleHit(Collider other)
    {
        if (currentHeldKeycard == null)
        {
            // Attempt to get the IDamageable component from the hit object or its parent
            IDamageable damageable = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(meleeDamage);
                HasHitDamagable = true;
            }
        }

        if (HasHitDamagable) return; //If we melee a enemy we cannot pick up interactable

        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(this);
            HasHitInteractble = true;
        }
    }
}
