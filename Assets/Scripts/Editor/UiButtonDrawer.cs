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

        // Validate the script and fetch methods
        if (script != null)
        {
            // Fetch valid methods: public instance methods with no parameters and void return type
            var methods = script.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
                .Select(m => m.Name)
                .Prepend("<None>") // Add a "None" option
                .ToArray();

            // Display the function dropdown
            int selectedIndex = System.Array.IndexOf(methods, functionNameProperty.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;

            Rect dropdownRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            selectedIndex = EditorGUI.Popup(dropdownRect, "Click Function", selectedIndex, methods);

            // Update the property value
            functionNameProperty.stringValue = selectedIndex > 0 ? methods[selectedIndex] : string.Empty; // Clear if "<None>" is selected
        }
        else
        {
            Rect errorRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(errorRect, "UiPage not found or no valid methods.");
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Adjust height to accommodate both fields
        return EditorGUIUtility.singleLineHeight * 2 + 4;
    }
}