using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private void OnSceneGUI()
    {
        Spawner spawner = (Spawner)target;

        // Get the Transform component of the Spawner
        Transform spawnerTransform = spawner.transform;

        // Convert local positions to world positions for the handles
        Vector3 worldStartPosition = spawnerTransform.TransformPoint(spawner.localStartPosition);
        Vector3 worldEndPosition = spawnerTransform.TransformPoint(spawner.localEndPosition);

        EditorGUI.BeginChangeCheck();

        // Create position handles for the start and end positions in world space
        Vector3 newWorldStartPosition = Handles.PositionHandle(worldStartPosition, Quaternion.identity);
        Vector3 newWorldEndPosition = Handles.PositionHandle(worldEndPosition, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spawner, "Move Spawner Points");

            // Convert the updated world positions back to local positions
            spawner.localStartPosition = spawnerTransform.InverseTransformPoint(newWorldStartPosition);
            spawner.localEndPosition = spawnerTransform.InverseTransformPoint(newWorldEndPosition);

            EditorUtility.SetDirty(spawner);
        }

        // Draw lines between the corners of the box in world space
        Handles.DrawLine(worldStartPosition, spawnerTransform.TransformPoint(new Vector3(spawner.localEndPosition.x, spawner.localStartPosition.y, spawner.localStartPosition.z)));
        Handles.DrawLine(worldStartPosition, spawnerTransform.TransformPoint(new Vector3(spawner.localStartPosition.x, spawner.localEndPosition.y, spawner.localStartPosition.z)));
        Handles.DrawLine(worldStartPosition, spawnerTransform.TransformPoint(new Vector3(spawner.localStartPosition.x, spawner.localStartPosition.y, spawner.localEndPosition.z)));

        Handles.DrawLine(worldEndPosition, spawnerTransform.TransformPoint(new Vector3(spawner.localStartPosition.x, spawner.localEndPosition.y, spawner.localEndPosition.z)));
        Handles.DrawLine(worldEndPosition, spawnerTransform.TransformPoint(new Vector3(spawner.localEndPosition.x, spawner.localStartPosition.y, spawner.localEndPosition.z)));
        Handles.DrawLine(worldEndPosition, spawnerTransform.TransformPoint(new Vector3(spawner.localEndPosition.x, spawner.localEndPosition.y, spawner.localStartPosition.z)));

        Handles.DrawLine(spawnerTransform.TransformPoint(new Vector3(spawner.localStartPosition.x, spawner.localEndPosition.y, spawner.localStartPosition.z)), spawnerTransform.TransformPoint(new Vector3(spawner.localEndPosition.x, spawner.localEndPosition.y, spawner.localStartPosition.z)));
        Handles.DrawLine(spawnerTransform.TransformPoint(new Vector3(spawner.localStartPosition.x, spawner.localStartPosition.y, spawner.localEndPosition.z)), spawnerTransform.TransformPoint(new Vector3(spawner.localEndPosition.x, spawner.localStartPosition.y, spawner.localEndPosition.z)));
        Handles.DrawLine(spawnerTransform.TransformPoint(new Vector3(spawner.localEndPosition.x, spawner.localStartPosition.y, spawner.localStartPosition.z)), spawnerTransform.TransformPoint(new Vector3(spawner.localEndPosition.x, spawner.localEndPosition.y, spawner.localStartPosition.z)));
        Handles.DrawLine(spawnerTransform.TransformPoint(new Vector3(spawner.localStartPosition.x, spawner.localEndPosition.y, spawner.localEndPosition.z)), spawnerTransform.TransformPoint(new Vector3(spawner.localStartPosition.x, spawner.localStartPosition.y, spawner.localEndPosition.z)));
    }
}