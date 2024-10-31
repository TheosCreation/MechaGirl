using System;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    public bool isHeld = false;
    public string colorTag = "Blue";

    private Rigidbody rb;
    [SerializeField] private Collider physicsCollider;
    [SerializeField] private Collider triggerCollider;
    private SpriteRenderer sRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        physicsCollider = GetComponent<Collider>();
        sRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerMeleeAndInteractions player = other.GetComponent<PlayerMeleeAndInteractions>();
        if (player != null && !isHeld)
        {
            physicsCollider.enabled = false;
            triggerCollider.enabled = false;
            sRenderer.enabled = false;

            rb.useGravity = false;
            rb.isKinematic = true;

            isHeld = true;

            player.AddKey(this);
        }
    }
}