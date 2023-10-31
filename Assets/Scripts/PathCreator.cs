using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Path
{
    [ExecuteInEditMode]
    public class PathCreator : MonoBehaviour
    {
        [SerializeField] public Vector2[] controlPoints;
        [SerializeField, Range(1, 50)] private int _resolution = 4;

        private int pathSegmentCount => _resolution * (controlPoints.Length - 1);
        private Vector2[] _pathPoints;

        [SerializeField] private Color _controlPointColor = Color.green;
        [SerializeField] private Color _pathColor = Color.red;

        private void OnDrawGizmos()
        {
            if (controlPoints.Length >= 4)
            {
                Gizmos.color = _controlPointColor;
                for (int i = 0; i < controlPoints.Length; i++)
                {
                    Gizmos.DrawSphere(controlPoints[i].XZ(), 0.5f);
                }

                DrawBezierCurve(controlPoints);
            }
        }

        private void DrawBezierCurve(Vector2[] points)
        {
            Gizmos.color = _pathColor;
            _pathPoints = BezierCurve.GetBezierCurvePathPoints(points, pathSegmentCount);
            for (int i = 0; i < _pathPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(_pathPoints[i].XZ(), _pathPoints[i + 1].XZ());
            }
        }

        public Vector3[] GetPathPoints()
        {
            Vector3[] pathPoints = new Vector3[pathSegmentCount + 1];
            int i = 0;
            foreach (Vector2 pathPoint in BezierCurve.GetBezierCurvePathPoints(controlPoints, pathSegmentCount))
            {
                pathPoints[i++] = pathPoint.XZ();
            }
            return pathPoints;
        }
    }
}


