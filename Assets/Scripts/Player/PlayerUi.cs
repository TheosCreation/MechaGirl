using UnityEngine;

public class PlayerUi : MonoBehaviour
{
    void Awake()
    {
        InputManager.Instance.playerInput.Ui.Pause.started += _ctx => PauseManager.Instance.TogglePause();
    }
}
