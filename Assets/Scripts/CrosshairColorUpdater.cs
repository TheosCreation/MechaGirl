using UnityEngine;

public class CrosshairColorUpdater : MonoBehaviour
{
    public Material crosshairMaterial; // Material using the custom shader
    public RenderTexture renderTexture; // RenderTexture assigned to the camera

    void Start()
    {
        // Assign the RenderTexture to the shader's _MainTex property
        if (crosshairMaterial != null && renderTexture != null)
        {
            crosshairMaterial.SetTexture("_MainTex", renderTexture);
        }
    }

    void Update()
    {
        // The center UV coordinates are always (0.5, 0.5)
        Vector2 screenCenterUV = new Vector2(0.5f, 0.5f);
        crosshairMaterial.SetVector("_ScreenCenterUV", screenCenterUV);
    }
}
