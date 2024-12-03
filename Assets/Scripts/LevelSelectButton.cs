using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelToOpen = 1;
    private Button button;
    [SerializeField] private Image levelLockImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GameManager.Instance.TryOpenLevel(levelToOpen-1));

        if (GameManager.Instance.GameState.IsCurrentLevelLocked(levelToOpen-1))
        {
            levelLockImage.gameObject.SetActive(true);
        }
    }
}
