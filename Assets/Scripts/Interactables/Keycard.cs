using System;
using UnityEngine;

public class Keycard : MonoBehaviour, IInteractable
{
    public bool isHeld = false;
    public bool isPlaced = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider physicsCollider;
    [SerializeField] private SpriteRenderer sRenderer;

    private void Awake()
    {
        physicsCollider = GetComponent<Collider>();
        sRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Interact(PlayerMeleeAndInteractions _fromPlayer)
    {
        if (isHeld || isPlaced) return;
        _fromPlayer.currentHeldKeycard = this;

        rb.useGravity = false;
        rb.isKinematic = true;

        physicsCollider.enabled = false;
        sRenderer.enabled = false;

        transform.parent = _fromPlayer.interactableTransform;
        transform.localPosition = Vector3.zero;

        isHeld = true;
    }

    public void Place(Transform keycardHolderTransform)
    {
        transform.parent = keycardHolderTransform;
        transform.localPosition = Vector3.zero;

        isHeld = false;
        isPlaced = true;
        physicsCollider.enabled = true;
        sRenderer.enabled = true;
    }

    public void Throw(float throwForce, Vector3 direction)
    {
        Debug.Log("Called");
        transform.parent = null;

        rb.useGravity = true;
        rb.isKinematic = false;

        physicsCollider.enabled = true;
        sRenderer.enabled = true;

        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);

        isHeld = false;
    }
}