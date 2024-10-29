using UnityEngine;

public class PlayerMeleeAndInteractions : MonoBehaviour
{
    public bool canMelee = true;
    [SerializeField] private float meleeDistance = 2.0f;
    [SerializeField] private float meleeDamage = 10.0f;
    [SerializeField] private float meleeCooldown = 1.5f;
    [SerializeField] private LayerMask hitMask;
    private PlayerController playerController;
    private Timer meleeTimer;
    private bool HasHitDamagable = false;

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

        // Perform a sphere cast to detect all objects within melee range
        RaycastHit[] hits = Physics.RaycastAll(transform.position, playerController.playerCamera.transform.forward, meleeDistance, hitMask);
        foreach (RaycastHit hit in hits)
        {
            // Check if the hit object has an IDamageable component and apply damage if it does
            HandleHit(hit.collider);
        }

        HasHitDamagable = false;
        canMelee = false;
        meleeTimer.SetTimer(meleeCooldown, () => canMelee = true);
    }

    private void HandleHit(Collider other)
    {
        // Attempt to get the IDamageable component from the hit object or its parent
        IDamageable damageable = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(meleeDamage);
            HasHitDamagable = true;
        }

        if (HasHitDamagable) return; //If we melee a enemy we cannot pick up interactable

        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(playerController);
        }
    }
}
