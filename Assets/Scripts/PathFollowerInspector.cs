using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UNV.Path
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
            if (GUILayout.Button("Start Motion"))
            {
                _pathFollower.StartMotion();
            }
            if (GUILayout.Button("Pause Motion"))
            {
                _pathFollower.PauseMotion();
            }
            if (GUILayout.Button("Resume Motion"))
            {
                _pathFollower.ResumeMotion();
            }
        }
    }
}