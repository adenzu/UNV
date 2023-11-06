using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UNV.Path2D
{
    [CustomEditor(typeof(PathFollower))]
    public class PathFollowerInspector : Editor
    {
        private PathFollower _pathFollower;

        private void OnEnable()
        {
            _pathFollower = (PathFollower)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Play Motion"))
            {
                _pathFollower.PlayMotion();
            }
            if (GUILayout.Button("Pause Motion"))
            {
                _pathFollower.PauseMotion();
            }
            if (GUILayout.Button("Update Path"))
            {
                _pathFollower.UpdatePath();
            }
        }
    }
}