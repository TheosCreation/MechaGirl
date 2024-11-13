using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways] // Makes the script run in both Edit and Play modes
[RequireComponent(typeof(VerticalLayoutGroup))]
public class ResponsiveSpacing : MonoBehaviour
{
    public float spacingMultiplier = 0.02f; // Adjust this value to control responsiveness
    private VerticalLayoutGroup layoutGroup;
    private RectTransform parentRectTransform;

    private void OnEnable()
    {
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        parentRectTransform = transform.parent?.GetComponent<RectTransform>();

        // Adjust spacing immediately when enabled
        AdjustSpacing();
    }

    private void Update()
    {
        // Adjust in edit mode or when changes are detected in play mode
        if (!Application.isPlaying || parentRectTransform.hasChanged)
        {
            AdjustSpacing();
            parentRectTransform.hasChanged = false; // Reset change flag
        }
    }

    private void AdjustSpacing()
    {
        if (layoutGroup == null || parentRectTransform == null)
            return;

        // Adjust the spacing based on the parent's height
        float height = parentRectTransform.rect.height;
        layoutGroup.spacing = height * spacingMultiplier;
    }
}
