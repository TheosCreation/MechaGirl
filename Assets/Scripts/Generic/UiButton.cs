using System;
using UnityEngine.UI;

[Serializable]
public struct UiButton
{
    public Button button;
    public string clickFunction;  // Stores the method name to call
    public string[] parameters;
}
