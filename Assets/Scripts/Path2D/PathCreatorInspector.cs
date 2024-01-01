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
            for (int i = 0; i < _pathCreator.controlPoints.Length; i++)
            {
                Vector3 controlPoint = _pathCreator.controlPoints[i];
                EditorGUI.BeginChangeCheck();
                controlPoint = Handles.PositionHandle(controlPoint, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_pathCreator, "Move Point");
                    _pathCreator.controlPoints[i] = controlPoint;
                }
            }
        }
    }
}