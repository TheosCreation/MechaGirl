using UnityEngine;

public class KeyLockedDoor : Door
{
    [SerializeField] private Color[] keysColorTagsToUnlock;
    [SerializeField] private GameObject doorLockOverlay;
    [SerializeField] private AudioSource doorUnlockSource;
    [SerializeField] private AudioClip doorUnlockClip;

    protected override void Open(Collider other)
    {
        PlayerMeleeAndInteractions player = other.GetComponent<PlayerMeleeAndInteractions>();
        if (player != null)
        {
            if(player.Holds(keysColorTagsToUnlock))
            {
                if (doorLockOverlay != null && doorLockOverlay.activeSelf)
                {
                    doorUnlockSource.PlayOneShot(doorUnlockClip);
                    doorLockOverlay.SetActive(false);
                }
                base.Open(other);
            }
        }
    }
}