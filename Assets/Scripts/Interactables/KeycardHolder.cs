using UnityEngine;
using UnityEngine.Events;

public class KeycardHolder : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform keycardHolderTransform;

    public UnityEvent keycardPlaced;

    public void Interact(PlayerMeleeAndInteractions _fromPlayer)
    {
        if(_fromPlayer.currentHeldKeycard != null)
        {
            _fromPlayer.currentHeldKeycard.Place(keycardHolderTransform);
            keycardPlaced?.Invoke();
        }
    }
}