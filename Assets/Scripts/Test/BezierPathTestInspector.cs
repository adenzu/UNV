using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierPathTest))]
public class BezierPathTestInspector : Editor
{
    private BezierPathTest _bezierPathTest;

    private void OnEnable()
    {
        _bezierPathTest = (BezierPathTest)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create Points"))
        {
            _bezierPathTest.DeletePoints();
            _bezierPathTest.CreatePoints();
        }

        if (GUILayout.Button("Calculate Path"))
        {
            _bezierPathTest.CalculatePath();
        }
    }
}
