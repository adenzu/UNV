using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Path2D;

public class BoatControllerPathCreator : MonoBehaviour
{
    [SerializeField] private BoatController _boatController;
    [SerializeField] private PathCreator _pathCreator;
    [SerializeField] private bool _loop = true;

    private int _currentWaypointIndex = 0;

    private void Start()
    {
        _boatController.OnArrivalToDestinationHook += OnArrivalToDestination;
    }

    public void StartPath(bool moveToStart = false)
    {
        _currentWaypointIndex = 0;
        if (moveToStart)
        {
            transform.position = _pathCreator.Waypoints[_currentWaypointIndex];
            transform.localRotation = Quaternion.identity;
        }
        SetDestination();
    }

    public void SetDestination()
    {
        _boatController.SetDestination(_pathCreator.Waypoints[_currentWaypointIndex]);
    }

    private void OnArrivalToDestination()
    {
        _currentWaypointIndex++;
        if (_currentWaypointIndex >= _pathCreator.Waypoints.Count)
        {
            if (_loop)
            {
                _currentWaypointIndex = 0;
                SetDestination();
            }
            else
            {
                _currentWaypointIndex = _pathCreator.Waypoints.Count - 1;
            }
        }
        else
        {
            SetDestination();
        }
    }
}
