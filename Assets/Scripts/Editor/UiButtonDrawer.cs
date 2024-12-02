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
        var parametersProperty = property.FindPropertyRelative("parameters");

        // Display the Button field
        Rect buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(buttonRect, buttonProperty);

        // Get the target object (UiPage) to find its methods
        var script = property.serializedObject.targetObject as UiPage;

        // Validate the script and fetch methods
        if (script != null)
        {
            // Fetch valid methods: public/non-public instance methods defined in the specific script type
            var methods = script.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) // Exclude inherited methods
                .Where(m => m.ReturnType == typeof(void)) // Only void return type
                .Select(m => new { m.Name, Parameters = m.GetParameters() })
                .Prepend(new { Name = "<None>", Parameters = new ParameterInfo[0] }) // Add a "None" option
                .ToArray();

            // Display the function dropdown
            string[] methodNames = methods.Select(m => m.Name).ToArray();
            int selectedIndex = System.Array.IndexOf(methodNames, functionNameProperty.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;

            Rect dropdownRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            selectedIndex = EditorGUI.Popup(dropdownRect, "Click Function", selectedIndex, methodNames);

            // Update the property value
            functionNameProperty.stringValue = selectedIndex > 0 ? methods[selectedIndex].Name : string.Empty;

            // If the selected method has parameters, show input fields
            var selectedMethod = methods[selectedIndex];
            if (selectedMethod.Parameters.Length > 0)
            {
                float yOffset = EditorGUIUtility.singleLineHeight * 2 + 4;
                for (int i = 0; i < selectedMethod.Parameters.Length; i++)
                {
                    var param = selectedMethod.Parameters[i];
                    Rect paramRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);

                    // Ensure the parameters array size matches the number of method parameters
                    if (parametersProperty.arraySize <= i)
                    {
                        parametersProperty.InsertArrayElementAtIndex(i);
                    }

                    SerializedProperty paramProperty = parametersProperty.GetArrayElementAtIndex(i);
                    paramProperty.stringValue = EditorGUI.TextField(paramRect, $"{param.Name} ({param.ParameterType.Name})", paramProperty.stringValue);
                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                }

                // Remove extra array elements
                while (parametersProperty.arraySize > selectedMethod.Parameters.Length)
                {
                    parametersProperty.DeleteArrayElementAtIndex(parametersProperty.arraySize - 1);
                }
            }
            else
            {
                // Clear parameters if the method takes no arguments
                parametersProperty.ClearArray();
            }
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
        var functionNameProperty = property.FindPropertyRelative("clickFunction");
        var parametersProperty = property.FindPropertyRelative("parameters");

        // Get the target object to check method parameters
        var script = property.serializedObject.targetObject as UiPage;
        if (script != null && !string.IsNullOrEmpty(functionNameProperty.stringValue))
        {
            var method = script.GetType()
                .GetMethod(functionNameProperty.stringValue, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            if (method != null)
            {
                // Add height for parameters
                return EditorGUIUtility.singleLineHeight * (2 + method.GetParameters().Length) + 6;
            }
        }

        // Default height for button and dropdown
        return EditorGUIUtility.singleLineHeight * 2 + 4;
    }
}
