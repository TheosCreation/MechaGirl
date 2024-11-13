using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    private TMP_Text m_textBox;
    [SerializeField] private int levelToOpen = 1;
    private Button button;
    [SerializeField] private Image levelLockImage;

    private void Awake()
    {
        m_textBox = GetComponentInChildren<TMP_Text>();
        m_textBox.text = levelToOpen.ToString();

        button = GetComponent<Button>();
        button.onClick.AddListener(() => GameManager.Instance.TryOpenLevel(levelToOpen-1));

        if (GameManager.Instance.GameState.IsCurrentLevelLocked(levelToOpen-1))
        {
            levelLockImage.gameObject.SetActive(true);
        }
    }
}
