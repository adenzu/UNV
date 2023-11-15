using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _objectToSpawn;
    [SerializeField, Min(0)] private int _numberOfObjectsToSpawn;
    [SerializeField, Min(0)] private float _externalRadius;
    [SerializeField, Min(0)] private float _internalRadius;

    private float _previousExternalRadius;
    private float _previousInternalRadius;

    public void RespawnObjects()
    {
        DespawnObjects();
        SpawnObjects();
        UpdateRadii();
    }

    public void SpawnObjects()
    {
        for (int i = 0; i < _numberOfObjectsToSpawn; i++)
        {
            GameObject spawnedObject = Instantiate(_objectToSpawn, transform);
            spawnedObject.transform.localPosition = GetRandomSpawnPosition();
        }
    }

    public void RearrangeObjects()
    {
        float previousRadiusDifference = _previousExternalRadius - _previousInternalRadius;
        float radiusDifference = _externalRadius - _internalRadius;
        float changeRatio = radiusDifference / previousRadiusDifference;
        foreach (Transform spawnedObject in transform)
        {
            Vector3 difference = spawnedObject.localPosition;
            float distance = difference.magnitude;
            spawnedObject.localPosition = ((distance - _previousInternalRadius) * changeRatio + _internalRadius) * difference.normalized;
        }
        UpdateRadii();
    }

    public void DespawnObjects()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => DestroyImmediate(child));
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 vector = Random.insideUnitCircle.XZ();
        return vector * (_externalRadius - _internalRadius) + vector.normalized * _internalRadius;
    }

    private void UpdateRadii()
    {
        _previousExternalRadius = _externalRadius;
        _previousInternalRadius = _internalRadius;
    }
}
