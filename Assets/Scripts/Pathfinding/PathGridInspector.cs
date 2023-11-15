using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGrid))]
public class PathGridInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PathGrid pathGrid = (PathGrid)target;

        if (GUILayout.Button("Create Grid"))
        {
            pathGrid.CreateGrid();
        }
    }
}
