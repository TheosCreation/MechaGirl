using System.Linq;
using UnityEngine;

public class PlayerMeleeAndInteractions : MonoBehaviour
{
    public bool canMelee = true;
    public Transform interactableTransform;
    [SerializeField] private float meleeDistance = 2.0f;
    [SerializeField] private float meleeDamage = 10.0f;
    [SerializeField] private float meleeCooldown = 1.5f;
    [SerializeField] private LayerMask hitMask;
    private PlayerController playerController;
    private Timer meleeTimer;
    private bool HasHitDamagable = false;
    private bool HasHitInteractble = false;
    private void Awake()
    {

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
        }

        HasHitDamagable = false;
        HasHitInteractble = false;
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
            interactable.Interact(this);
            HasHitInteractble = true;
        }
    }

    public bool Holds(Color[] keysToUnlock)
    {
        // Check if each key's color tag in keysToUnlock is present in currentHeldKeycards
        foreach (Color keyColorTag in keysToUnlock)
        {
            bool hasMatchingKey = LevelManager.Instance.currentHeldKeycards
                .Any(card => ColorsAreEqual(card.colorTag, keyColorTag));

            if (!hasMatchingKey)
            {
                Debug.Log($"Missing key for color: {keyColorTag}");
                return false;
            }
        }

        return true;
    }

    // Helper method for color comparison with a tolerance
    private bool ColorsAreEqual(Color color1, Color color2, float tolerance = 0.01f)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance &&
               Mathf.Abs(color1.a - color2.a) < tolerance;
    }

    public void AddKey(Keycard keycard)
    {
        keycard.transform.parent = interactableTransform;
        keycard.transform.localPosition = Vector3.zero;

        LevelManager.Instance.currentHeldKeycards.Add(keycard);
        UiManager.Instance.AddKeyCardIcon(keycard);
    }

    
}
