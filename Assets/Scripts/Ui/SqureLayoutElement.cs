using UnityEngine;

[ExecuteAlways] // Makes the script run in edit mode too
public class SquareLayoutElement : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent?.GetComponent<RectTransform>();
    }

    void Update()
    {
        AdjustWidthToHeight();
    }

    private void AdjustWidthToHeight()
    {
        if (parentRectTransform == null)
            return;

        // Calculate the height based on the parent's height and make it square
        float height = parentRectTransform.rect.height;
        rectTransform.sizeDelta = new Vector2(height, height);
    }
}