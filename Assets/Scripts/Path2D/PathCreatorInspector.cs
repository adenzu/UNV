using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UNV.Path2D
{
    [CustomEditor(typeof(PathCreator))]
    public class PathCreatorInspector : Editor
    {
        private PathCreator _pathCreator;

        private void OnEnable()
        {
            _pathCreator = (PathCreator)target;
        }

        private void OnSceneGUI()
        {
            for (int i = 0; i < _pathCreator.Waypoints.Count; i++)
            {
                Vector3 waypoint = _pathCreator.Waypoints[i];
                EditorGUI.BeginChangeCheck();
                waypoint = Handles.PositionHandle(waypoint, Quaternion.identity);
                Handles.Label(waypoint, $"Control Point {i}");
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_pathCreator, "Move Point");
                    _pathCreator.Waypoints[i] = waypoint;
                }
            }
        }
    }
}