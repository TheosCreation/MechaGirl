using UnityEngine;
using static Unity.VisualScripting.Member;

public class Keycard : MonoBehaviour
{
    public bool isHeld = false;
    public Color colorTag = Color.blue;

    private Rigidbody rb;
    [SerializeField] private Collider physicsCollider;
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private AudioClip[] keyPickUpSounds;
    private AudioSource source;
    private SpriteRenderer sRenderer;
    public GameObject prefabIcon;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        physicsCollider = GetComponent<Collider>();
        sRenderer = GetComponentInChildren<SpriteRenderer>();
        source = GetComponent<AudioSource>();
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
            PlayRandomPickUpSound();
        }
    }

    private void PlayRandomPickUpSound()
    {
        if (keyPickUpSounds.Length == 0) return; // Exit if there are no sounds available

        // Pick a random sound from the array
        int randomIndex = Random.Range(0, keyPickUpSounds.Length);
        AudioClip randomClip = keyPickUpSounds[randomIndex];

        // Play the selected sound
        source.PlayOneShot(randomClip);
    }
}