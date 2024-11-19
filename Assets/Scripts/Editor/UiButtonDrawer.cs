using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UiButton))]
public class UiButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the UiButton fields
        var buttonProperty = property.FindPropertyRelative("button");
        var functionNameProperty = property.FindPropertyRelative("clickFunction");

        // Display the Button field
        Rect buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(buttonRect, buttonProperty);

        // Get the target object (UiPage) to find its methods
        var script = property.serializedObject.targetObject as UiPage;
        var methods = script?.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
            .Select(m => m.Name)
            .ToArray();

        if (methods != null)
        {
            // Display the function dropdown
            int selectedIndex = System.Array.IndexOf(methods, functionNameProperty.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;

            Rect dropdownRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            selectedIndex = EditorGUI.Popup(dropdownRect, "Function", selectedIndex, methods);

            functionNameProperty.stringValue = methods[selectedIndex];
        }
        else
        {
            EditorGUI.LabelField(position, "UiPage not found or has no valid methods");
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + 4;
    }
}
