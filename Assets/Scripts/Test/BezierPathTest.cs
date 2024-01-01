using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BezierPathTest : MonoBehaviour
{
    [SerializeField, Min(0)] private int _pointCount = 10;
    [SerializeField, Min(0)] private int _resolution = 2;
    [SerializeField, Min(0)] private float _weight = 1;

    private Transform _pointsParent;

    private int[] _binomials;

    private Vector3[] _pathPoints;

    private void Awake()
    {
        _pointsParent = new GameObject("Points").transform;
    }

    public void DeletePoints()
    {
        if (_pointsParent == null)
        {
            return;
        }

        DestroyImmediate(_pointsParent.gameObject);
        _pointsParent = new GameObject("Points").transform;
    }

    public void CreatePoints()
    {
        if (_pointsParent == null)
        {
            return;
        }

        for (int i = 0; i < _pointCount; i++)
        {
            GameObject point = new GameObject($"Point {i}");
            point.transform.SetParent(_pointsParent);
            point.transform.localPosition = Vector3.right * i;
        }

        CalculateBinomials();
    }

    public void CalculatePath()
    {
        if (_pointsParent == null)
        {
            return;
        }

        WeightedBezier();
    }

    private void OnDrawGizmos()
    {
        if (_pointsParent == null) return;

        for (int i = 0; i < _pointsParent.childCount; i++)
        {
            Gizmos.DrawSphere(_pointsParent.GetChild(i).position, 0.1f);
        }

        if (_pathPoints == null) return;

        for (int i = 0; i < _pathPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(_pathPoints[i], _pathPoints[i + 1]);
        }
    }

    private void WeightedBezier()
    {
        _pathPoints = new Vector3[_resolution * (_pointCount - 1) + 1];
        _pathPoints[0] = _pointsParent.GetChild(0).localPosition;
        _pathPoints[_pathPoints.Length - 1] = _pointsParent.GetChild(_pointCount - 1).localPosition;
        for (int i = 1; i < _pathPoints.Length - 1; i++)
        {
            float t = (float)i / (_pathPoints.Length - 1);
            _pathPoints[i] = CalculatePoint(t);
        }
    }

    private Vector3 CalculatePoint(float t)
    {
        Vector3 point = Vector3.zero;
        float divisor = 0;

        for (int i = 0; i < _pointCount; i++)
        {
            float deltaDivisor = GetWeight(i) * _binomials[i] * Mathf.Pow(t, i) * Mathf.Pow(1 - t, _pointCount - i - 1);
            divisor += deltaDivisor;
            point += deltaDivisor * _pointsParent.GetChild(i).localPosition;
        }

        return point / divisor;
    }


    private void CalculateBinomials()
    {
        _binomials = new int[_pointCount];
        _binomials[0] = 1;

        for (int i = 1; i < _pointCount; i++)
        {
            _binomials[i] = _binomials[i - 1] * (_pointCount - i) / i;
        }
    }

    private float GetWeight(int i)
    {
        if (i == 0) return 1;
        if (i == _pointCount - 1) return 1;
        return _weight;
    }
}
