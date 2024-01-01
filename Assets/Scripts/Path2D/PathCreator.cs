using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Path;

namespace UNV.Path2D
{
    [ExecuteInEditMode]
    public class PathCreator : MonoBehaviour
    {
        [SerializeField] public Vector3[] controlPoints;
        [SerializeField, Range(1, 50)] private int _resolution = 4;

        private Vector3[] _pathPoints;

        [SerializeField] private Color _controlPointColor = Color.green;
        [SerializeField] private Color _pathColor = Color.red;

        private void OnDrawGizmosSelected()
        {
            if (controlPoints.Length >= 4)
            {
                Gizmos.color = _controlPointColor;
                for (int i = 0; i < controlPoints.Length; i++)
                {
                    Gizmos.DrawSphere(controlPoints[i], 0.5f);
                }

                DrawBezierCurve(controlPoints);
            }
        }

        private void DrawBezierCurve(Vector3[] points)
        {
            if (_pathPoints == null) return;
            Gizmos.color = _pathColor;
            for (int i = 0; i < _pathPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(_pathPoints[i], _pathPoints[i + 1]);
            }
        }

        [ContextMenu("Update Path")]
        public void UpdatePath()
        {
            _pathPoints = PathProcessing.GetBezierPath(controlPoints, _resolution);
        }

        public Vector3[] GetPathPoints()
        {
            return _pathPoints;
        }
    }
}


