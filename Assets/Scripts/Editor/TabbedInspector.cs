using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)] // Specify the target class if not MonoBehaviour
    public class TabbedInspector : UnityEditor.Editor
    {
        private int selectedTab;
        private List<string> tabHeaders;
        private Dictionary<string, List<SerializedProperty>> tabProperties;

        private void OnEnable()
        {
            tabHeaders = new List<string>();
            tabProperties = new Dictionary<string, List<SerializedProperty>>();

            SerializedProperty property = serializedObject.GetIterator();
            string currentHeader = null;

            do
            {
                if (property.depth > 0)
                    continue;

                var attributes = GetPropertyAttributes<Runtime.TabAttribute>(property);
                if (attributes.Length > 0)
                {
                    currentHeader = attributes[0].tabName;
                    if (!tabHeaders.Contains(currentHeader))
                    {
                        tabHeaders.Add(currentHeader);
                        tabProperties[currentHeader] = new List<SerializedProperty>();
                    }
                }

                if (currentHeader != null)
                {
                    tabProperties[currentHeader].Add(property.Copy());
                }

            } while (property.NextVisible(true));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (tabHeaders.Count > 0)
            {
                selectedTab = GUILayout.Toolbar(selectedTab, tabHeaders.ToArray());

                if (selectedTab >= 0 && selectedTab < tabHeaders.Count)
                {
                    string header = tabHeaders[selectedTab];
                    foreach (SerializedProperty property in tabProperties[header])
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
            }
            else
            {
                base.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private T[] GetPropertyAttributes<T>(SerializedProperty property) where T : System.Attribute
        {
            FieldInfo fieldInfo = serializedObject.targetObject.GetType().GetField(property.propertyPath,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
            }

            return new T[0];
        }
    }
}