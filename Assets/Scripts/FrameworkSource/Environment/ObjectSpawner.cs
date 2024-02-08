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

    [SerializeField] private bool _randomizeSize;
    [SerializeField, Min(0.01f)] private float _minSizeScale;
    [SerializeField, Min(0.01f)] private float _maxSizeScale;

    private float _previousExternalRadius;
    private float _previousInternalRadius;

    public void RespawnObjects()
    {
        DespawnObjects();
        SpawnObjects();
        UpdateRadii();
    }

    public void SetObjectCount(int count)
    {
        _numberOfObjectsToSpawn = count;
    }

    public void SetObjectCount(float count)
    {
        _numberOfObjectsToSpawn = (int)count;
    }

    public void SetExternalRadius(float radius)
    {
        _previousExternalRadius = _externalRadius;
        _externalRadius = radius;
    }

    public void SetInternalRadius(float radius)
    {
        _previousInternalRadius = _internalRadius;
        _internalRadius = radius;
    }

    public void SetRandomizeSize(bool randomizeSize)
    {
        _randomizeSize = randomizeSize;
    }

    public void SetMinSizeScale(float minSizeScale)
    {
        _minSizeScale = minSizeScale;
    }

    public void SetMaxSizeScale(float maxSizeScale)
    {
        _maxSizeScale = maxSizeScale;
    }

    public void SpawnObjects()
    {
        for (int i = 0; i < _numberOfObjectsToSpawn; i++)
        {
            GameObject spawnedObject = Instantiate(_objectToSpawn, transform);
            spawnedObject.transform.localPosition = GetRandomSpawnPosition();
            if (_randomizeSize)
            {
                spawnedObject.transform.localScale *= Random.Range(_minSizeScale, _maxSizeScale);
            }
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
            Vector3 newPosition = ((distance - _previousInternalRadius) * changeRatio + _internalRadius) * difference.normalized;
            spawnedObject.localPosition = new Vector3(newPosition.x, difference.y, newPosition.z);
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
