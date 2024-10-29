using UnityEngine;
using System.Collections;

public class Casing : MonoBehaviour
{
    [Header("Force X")]
    public float minimumXForce = 25f;
    public float maximumXForce = 40f;

    [Header("Force Y")]
    public float minimumYForce = 10f;
    public float maximumYForce = 20f;

    [Header("Force Z")]
    public float minimumZForce = -12f;
    public float maximumZForce = 12f;

    [Header("Despawn Time")]
    public float despawnTime = 10f;

    [Header("Audio")]
    public AudioClip[] casingSounds;
    public AudioSource audioSource;

    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Random direction the casing will be ejected in
        rb.AddRelativeForce(
            Random.Range(minimumXForce, maximumXForce),  // X Axis
            Random.Range(minimumYForce, maximumYForce),  // Y Axis
            Random.Range(minimumZForce, maximumZForce)); // Z Axis
    }

    private void Start()
    {
        // Destroy casings after the despawn time
        Destroy(gameObject, despawnTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlaySound();
    }

    private void PlaySound()
    {
        if (casingSounds.Length > 0)
        {
            // Get a random casing sound from the array
            audioSource.clip = casingSounds[Random.Range(0, casingSounds.Length)];

            // Play immediately when the casing hits the ground
            audioSource.Play();
        }
    }
}