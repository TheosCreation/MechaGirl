using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponImage : MonoBehaviour
{
    RectTransform rectTransform;
    float originalY;
    public float amplitude = 50f; 
    public float frequency = 1f; 

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalY = rectTransform.anchoredPosition.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float newY = originalY + Mathf.Sin(Time.time * frequency) * amplitude;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newY);
    }
}
