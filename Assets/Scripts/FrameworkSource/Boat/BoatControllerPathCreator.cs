using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Path2D;

public class BoatControllerPathCreator : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] private BoatController _boatController;
    [SerializeField] private PathCreator _pathCreator;
    [SerializeField] private bool _loop = true;
=======
    [SerializeField] private BoatController boatController;
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool moveAtSceneStart = true;
>>>>>>> Stashed changes

    private int _currentWaypointIndex = 0;

    private void Start()
    {
<<<<<<< Updated upstream
        _boatController.OnArrivalToDestinationHook += OnArrivalToDestination;
    }

    public void StartPath(bool moveToStart = false)
    {
        _currentWaypointIndex = 0;
        if (moveToStart)
        {
            transform.position = _pathCreator.Waypoints[_currentWaypointIndex];
=======
        boatController.OnArrivalToDestinationHook += OnArrivalToDestination;
        if (moveAtSceneStart)
        {
            StartPath();
        }
    }

    public void StartPath(bool teleportToStart = false)
    {
        _currentWaypointIndex = 0;
        if (teleportToStart)
        {
            transform.position = pathCreator.Waypoints[_currentWaypointIndex];
>>>>>>> Stashed changes
            transform.localRotation = Quaternion.identity;
        }
        SetDestination();
    }

    public void SetDestination()
    {
<<<<<<< Updated upstream
        _boatController.SetDestination(_pathCreator.Waypoints[_currentWaypointIndex]);
=======
        boatController.SetDestination(pathCreator.Waypoints[_currentWaypointIndex]);
>>>>>>> Stashed changes
    }

    private void OnArrivalToDestination()
    {
        _currentWaypointIndex++;
<<<<<<< Updated upstream
        if (_currentWaypointIndex >= _pathCreator.Waypoints.Count)
        {
            if (_loop)
=======
        if (_currentWaypointIndex >= pathCreator.Waypoints.Count)
        {
            if (loop)
>>>>>>> Stashed changes
            {
                _currentWaypointIndex = 0;
                SetDestination();
            }
            else
            {
<<<<<<< Updated upstream
                _currentWaypointIndex = _pathCreator.Waypoints.Count - 1;
=======
                _currentWaypointIndex = pathCreator.Waypoints.Count - 1;
>>>>>>> Stashed changes
            }
        }
        else
        {
            SetDestination();
        }
    }
}
