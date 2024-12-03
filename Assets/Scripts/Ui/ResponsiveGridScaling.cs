using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class ResponsiveLevelGrid : MonoBehaviour
{
    public Vector4 paddingPercentages; // Padding as percentages (Left, Top, Right, Bottom)
    public float spacingPercentage = 0.02f; // Spacing as a percentage of the parent size
    public float aspectRatio = 1f; // Aspect ratio of each cell (e.g., 1 for square)

    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;

    void OnEnable()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        AdjustGrid();
    }

    void Update()
    {
        if (!Application.isPlaying || rectTransform.hasChanged)
        {
            AdjustGrid();
            rectTransform.hasChanged = false; // Reset change flag
        }
    }

    void AdjustGrid()
    {
        if (gridLayoutGroup == null || rectTransform == null) return;

        Vector2 size = rectTransform.rect.size; // Get the size of the RectTransform

        // Adjust padding based on percentages
        gridLayoutGroup.padding.top = Mathf.RoundToInt(size.y * paddingPercentages.y);
        gridLayoutGroup.padding.bottom = Mathf.RoundToInt(size.y * paddingPercentages.w);
        gridLayoutGroup.padding.left = Mathf.RoundToInt(size.x * paddingPercentages.x);
        gridLayoutGroup.padding.right = Mathf.RoundToInt(size.x * paddingPercentages.z);

        // Adjust spacing based on percentage of parent size
        float spacing = Mathf.Min(size.x, size.y) * spacingPercentage;
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);

        // Calculate available size for cells
        float availableWidth = size.x - gridLayoutGroup.padding.horizontal;
        float availableHeight = size.y - gridLayoutGroup.padding.vertical;

        // Calculate cell size while maintaining the aspect ratio
        int columns = gridLayoutGroup.constraintCount;
        float cellWidth = availableWidth / columns - gridLayoutGroup.spacing.x;
        float cellHeight = cellWidth / aspectRatio;

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }
}