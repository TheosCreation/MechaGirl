using UnityEngine;

public class SpriteBillboardV2 : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
}