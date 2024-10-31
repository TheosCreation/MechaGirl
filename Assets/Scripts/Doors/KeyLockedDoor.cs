using UnityEngine;

public class KeyLockedDoor : Door
{
    [SerializeField] private string[] keysColorTagsToUnlock;

    protected override void Open(Collider other)
    {
        PlayerMeleeAndInteractions player = other.GetComponent<PlayerMeleeAndInteractions>();
        if (player != null)
        {
            if(player.Holds(keysColorTagsToUnlock))
            {
                base.Open(other);
            }
        }
    }
}