using UnityEngine;

public class RestartButton : MonoBehaviour
{
    private PlayerInput playerInput;
    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void Start()
    {
        playerInput.Ui.Restart.started += _ctx => LevelManager.Instance.RespawnPlayer();
    }

    void OnEnable()
    {
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }
}