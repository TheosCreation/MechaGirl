using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UiPage : MonoBehaviour
{
    [SerializeField] private UiButton[] buttons;

    // Cache for MethodInfo lookups
    private readonly Dictionary<string, MethodInfo> methodCache = new Dictionary<string, MethodInfo>();

    private void OnEnable()
    {
        if (buttons == null || buttons.Length == 0) return;

        foreach (UiButton uiButton in buttons)
        {
            if (uiButton.button != null && !string.IsNullOrEmpty(uiButton.clickFunction))
            {
                // Attempt to cache or retrieve the MethodInfo
                if (!methodCache.TryGetValue(uiButton.clickFunction, out MethodInfo method))
                {
                    method = GetType().GetMethod(uiButton.clickFunction, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    methodCache[uiButton.clickFunction] = method; // Cache the result
                }

                if (method != null)
                {
                    try
                    {
                        uiButton.button.onClick.AddListener(() => method.Invoke(this, null));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error invoking method '{uiButton.clickFunction}' on {nameof(UiPage)}: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Method '{uiButton.clickFunction}' not found on {nameof(UiPage)}");
                }
            }
        }
    }

    private void OnDisable()
    {
        if (buttons == null || buttons.Length == 0) return;

        foreach (UiButton uiButton in buttons)
        {
            uiButton.button?.onClick.RemoveAllListeners();
        }
    }
}
