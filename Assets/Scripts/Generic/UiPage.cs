using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UiPage : MonoBehaviour
{
    [SerializeField] protected UiButton[] buttons;

    // Cache for MethodInfo lookups
    private readonly Dictionary<string, MethodInfo> methodCache = new Dictionary<string, MethodInfo>();

    protected virtual void OnEnable()
    {
        if (buttons == null || buttons.Length == 0) return;

        foreach (UiButton uiButton in buttons)
        {
            if (uiButton.button != null && !string.IsNullOrEmpty(uiButton.clickFunction))
            {
                // Attempt to cache or retrieve the MethodInfo
                if (!methodCache.TryGetValue(uiButton.clickFunction, out MethodInfo method))
                {
                    method = GetType().GetMethod(
                        uiButton.clickFunction,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy
                    );

                    if (method != null)
                    {
                        methodCache[uiButton.clickFunction] = method; // Cache the result
                    }
                    else
                    {
                        Debug.LogWarning($"Method '{uiButton.clickFunction}' not found in {GetType().Name} or its base classes.");
                        continue; // Skip this button if no valid method is found
                    }
                }

                // Add the listener if the method was found
                try
                {
                    uiButton.button.onClick.AddListener(() => method.Invoke(this, null));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error invoking method '{uiButton.clickFunction}' on {nameof(UiPage)}: {ex.Message}");
                }
            }
        }
    }
    protected virtual void OnDisable()
    {
        if (buttons == null || buttons.Length == 0) return;

        foreach (UiButton uiButton in buttons)
        {
            uiButton.button?.onClick.RemoveAllListeners();
        }
    }
}
