using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Path;

namespace UNV.Path2D
{
    [ExecuteInEditMode]
    public class PathCreator : MonoBehaviour
    {
        [SerializeField] private List<Vector3> _waypoints = new();

        [SerializeField] private Color _controlPointColor = Color.green;

        public List<Vector3> Waypoints => _waypoints;

        public Vector3 AddWaypoint(Vector3 waypoint)
        {
            _waypoints.Add(waypoint);
            return waypoint;
        }

        public Vector3 RemoveWaypoint(Vector3 waypoint)
        {
            _waypoints.Remove(waypoint);
            return waypoint;
        }

        public Vector3 RemoveWaypoint(int index)
        {
            Vector3 waypoint = _waypoints[index];
            _waypoints.RemoveAt(index);
            return waypoint;
        }

        public Vector3 RemoveWaypoint()
        {
            if (_waypoints.Count > 0)
            {
                Vector3 waypoint = _waypoints[_waypoints.Count - 1];
                _waypoints.RemoveAt(_waypoints.Count - 1);
                return waypoint;
            }
            return Vector3.zero;
        }

        public void ClearWaypoints()
        {
            _waypoints.Clear();
        }
    }
}


