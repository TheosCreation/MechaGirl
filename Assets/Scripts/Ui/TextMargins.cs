using UnityEngine;
using TMPro;

[ExecuteAlways] // Makes the script run in both Edit and Play modes
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMP_Text))]
public class TextMargins : MonoBehaviour
{
    public Vector4 marginPercentage; // Margins as a percentage of the RectTransform size (Left, Top, Right, Bottom)

    private TMP_Text tmpText;
    private RectTransform rectTransform;

    void OnEnable()
    {
        tmpText = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Adjust in edit mode or when changes are detected in play mode
        if (!Application.isPlaying || rectTransform.hasChanged)
        {
            AdjustMargins();
            rectTransform.hasChanged = false; // Reset change flag
        }
    }

    void AdjustMargins()
    {
        if (tmpText == null || rectTransform == null) return;

        Vector2 size = rectTransform.rect.size; // Get the size of the RectTransform

        // Calculate margins as a percentage of the size
        tmpText.margin = new Vector4(
            size.x * marginPercentage.x, // Left
            size.y * marginPercentage.y, // Top
            size.x * marginPercentage.z, // Right
            size.y * marginPercentage.w  // Bottom
        );
    }
}

