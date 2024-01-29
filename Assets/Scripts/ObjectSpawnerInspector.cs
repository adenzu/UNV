using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerInspector : Editor
{
    ObjectSpawner objectSpawner;
    SerializedProperty randomizeSizeProperty;

    private void OnEnable()
    {
        objectSpawner = (ObjectSpawner)target;
        randomizeSizeProperty = serializedObject.FindProperty("_randomizeSize");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_objectToSpawn"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_numberOfObjectsToSpawn"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_externalRadius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_internalRadius"));
        EditorGUILayout.PropertyField(randomizeSizeProperty);

        if (randomizeSizeProperty.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_minSizeScale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxSizeScale"));
        }

        if (GUILayout.Button("Respawn"))
        {
            objectSpawner.RespawnObjects();
        }

        if (GUILayout.Button("Rearrange"))
        {
            objectSpawner.RearrangeObjects();
        }

        if (GUILayout.Button("Despawn"))
        {
            objectSpawner.DespawnObjects();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
