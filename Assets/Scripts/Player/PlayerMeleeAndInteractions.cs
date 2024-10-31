using System;
using System.Collections.Generic;
using System.Linq;
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
    public List<Keycard> currentHeldKeycards;

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

    public bool Holds(string[] keysToUnlock)
    {
        // Check if each key's color tag in keysToUnlock is present in currentHeldKeycards
        foreach (string keyColorTag in keysToUnlock)
        {
            bool hasMatchingKey = currentHeldKeycards.Any(card => card.colorTag == keyColorTag);
            if (!hasMatchingKey)
            {
                return false;
            }
        }

        return true;
    }

    public void AddKey(Keycard keycard)
    {
        keycard.transform.parent = interactableTransform;
        keycard.transform.localPosition = Vector3.zero;

        currentHeldKeycards.Add(keycard);
    }
}
