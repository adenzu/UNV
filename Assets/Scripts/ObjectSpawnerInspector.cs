using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerInspector : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        ObjectSpawner objectSpawner = (ObjectSpawner)target;
        VisualElement root = new VisualElement();

        root.Add(new PropertyField(serializedObject.FindProperty("_objectToSpawn")));
        root.Add(new PropertyField(serializedObject.FindProperty("_numberOfObjectsToSpawn")));
        root.Add(new PropertyField(serializedObject.FindProperty("_externalRadius")));
        root.Add(new PropertyField(serializedObject.FindProperty("_internalRadius")));

        Button spawnButton = new Button(objectSpawner.RespawnObjects);
        spawnButton.text = "Respawn";
        root.Add(spawnButton);

        Button rearrangeButton = new Button(objectSpawner.RearrangeObjects);
        rearrangeButton.text = "Rearrange";
        root.Add(rearrangeButton);

        Button despawnButton = new Button(objectSpawner.DespawnObjects);
        despawnButton.text = "Despawn";
        root.Add(despawnButton);

        return root;
    }
}
