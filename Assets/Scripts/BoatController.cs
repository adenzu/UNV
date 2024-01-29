using System;
using UnityEngine;
using UNV.Path;
using UNV.Pathfinding;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [SerializeField] private float _obstacleAvoidanceDistance = 5f;
    [SerializeField] private float _targetDeadZoneRadius = 0.1f;

    [SerializeField] private bool _useBezierPath = true;
    [SerializeField] private int _bezierPathNumSamples = 10;

    [SerializeField] private float _steerPower = 500f;
    [SerializeField] private float _power = 5f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _angularSpeedScale = 0.01f;

    [SerializeField] private float _steerCoefficient = 1f;

    [SerializeField] private TargetOvershootOptions _onTargetOvershoot = TargetOvershootOptions.DoNothing;

    [SerializeField] private ColregDetector _colregDetector;

    public delegate void OnArrivalToDestinationDelegate();
    public OnArrivalToDestinationDelegate OnArrivalToDestinationHook;

    private float _thrust;
    private float _rudderAngle;

    private Vector3 _destination;
    private Vector3[] _waypoints;
    private int _currentWaypointIndex, _previousWaypointIndex;
    private Vector3 _currentWaypoint => _waypoints != null && _waypoints.Length > 0 ? _waypointAdjuster(_waypoints[_currentWaypointIndex]) : transform.position;
    private Vector3 _previousWaypoint => _waypoints != null && _waypoints.Length > 0 ? _waypoints[_previousWaypointIndex] : transform.position;

    public Vector3 CurrentWaypoint => _currentWaypoint;

    private Func<Vector3, Vector3> _waypointAdjuster = (waypoint) => waypoint;
    private Func<Vector3, Vector3> _waypointDirectionAdjuster = (waypointDirection) => waypointDirection;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_colregDetector != null)
        {
            _colregDetector.OnColregStateChange += OnColregStateChange;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
        GridManager.Instance.GenerateGrid();
        DilateObstacles();
        RequestPath();
    }

    private void DilateObstacles()
    {
        if (!GridManager.Instance.DilatedBefore)
        {
            GridManager.Instance.DilateObstacles(Mathf.FloorToInt(_obstacleAvoidanceDistance / GridManager.Instance.NodeSize));
        }
    }

    private void RequestPath()
    {
        PathRequestManager.RequestPath(
            transform.position,
            _destination,
            _steerCoefficient * _steerPower / _maxSpeed,
            OnPathFound,
            () => Util.UnitSquare(transform.forward.XZ())
        );
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        if (!success)
        {
            GridManager.Instance.GenerateGrid();
            DilateObstacles();
            Invoke(nameof(RequestPath), 1f);
        }
        _waypoints = success ? (_useBezierPath ? PathProcessing.GetBezierPath(path, _bezierPathNumSamples) : path) : null;
        _currentWaypointIndex = 0;
    }

    private bool IsTargetReached()
    {
        float distanceToTarget = (_currentWaypoint - transform.position).magnitude;
        return distanceToTarget <= _targetDeadZoneRadius;
    }

    private void OnTargetReached()
    {
        if
        (
            _colregDetector.State == ColregDetector.ColregState.None ||
            _colregDetector.State == ColregDetector.ColregState.CrossingFromLeft
        )
        {
            _previousWaypointIndex = _currentWaypointIndex;
            _currentWaypointIndex++;
        }
        else
        {
            while
            (
                (_currentWaypointIndex < _waypoints.Length - 1) &&
                (Vector3.Dot(transform.forward, _waypoints[_currentWaypointIndex] - transform.position) < 0) &&
                ((_waypoints[_currentWaypointIndex] - transform.position).magnitude < _colregDetector.DetectionRadius)
            )
            {
                _previousWaypointIndex = _currentWaypointIndex;
                _currentWaypointIndex++;
            }
        }
        OnColregStateChange(ColregDetector.ColregState.None);
        if (IsArrivedToDestination())
        {
            OnArrivalToDestination();
        }
    }

    private bool IsTargetOvershoot()
    {
        if (Vector3.Distance(transform.position, _currentWaypoint) > _obstacleAvoidanceDistance * 1.5f)
        {
            return false;
        }
        Vector2 targetDirection = (_currentWaypoint - transform.position).XZ().normalized;
        Vector2 forward = transform.forward.XZ().normalized;
        float targetAngle = Vector2.Angle(targetDirection, forward);
        return targetAngle > 90f;
    }

    public void OnTargetOvershoot()
    {
        if (_onTargetOvershoot == TargetOvershootOptions.Skip)
        {
            OnTargetReached();
        }
    }

    private bool IsArrivedToDestination()
    {
        return _waypoints == null || _currentWaypointIndex >= _waypoints.Length;
    }

    private void OnArrivalToDestination()
    {
        _waypoints = null;
        _currentWaypointIndex = 0;
        _previousWaypointIndex = 0;
        OnArrivalToDestinationHook?.Invoke();
    }

    private void FixedUpdate()
    {
        if (_waypoints == null || _waypoints.Length == 0)
        {
            return;
        }

        if (ShouldWaitForPassage())
        {
            return;
        }

        Vector3 waypointDirection = _currentWaypoint - _previousWaypoint;

        if (_currentWaypointIndex == 0)
        {
            int nextWaypointIndex = _currentWaypointIndex + 1;
            Vector3 nextWaypoint = nextWaypointIndex < _waypoints.Length ? _waypoints[nextWaypointIndex] : _currentWaypoint + (_currentWaypoint - _previousWaypoint);
            waypointDirection = nextWaypoint - _currentWaypoint;
        }

        _thrust = 1f;
        _rudderAngle = FuzzyGuidanceController.GetCrispRudderAngle(
            transform.position,
            _currentWaypoint,
            transform.forward,
            _waypointDirectionAdjuster(waypointDirection)
        );

        Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward).normalized;

        Vector3 desiredForwardVelocity = forward * _maxSpeed * _thrust;
        float desiredAngularSpeed = _rudderAngle * _maxSpeed * _thrust * _angularSpeedScale;

        PhysicsHelper.ApplyAccelerationToReachAngularSpeed(_rigidbody, desiredAngularSpeed, _steerPower);
        PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, desiredForwardVelocity, _power);

        if (IsTargetReached())
        {
            OnTargetReached();
        }

        if (IsArrivedToDestination())
        {
            return;
        }

        if (IsTargetOvershoot())
        {
            OnTargetOvershoot();
        }
    }

    private void OnColregStateChange(ColregDetector.ColregState state)
    {
        Vector3 offset;
        switch (state)
        {
            case ColregDetector.ColregState.HeadOn:
                offset = (_colregDetector.TargetPosition + transform.position) / 2 + transform.right * (_obstacleAvoidanceDistance + _colregDetector.TargetRadius);
                _waypointAdjuster = (waypoint) => offset;
                _waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            case ColregDetector.ColregState.Overtaking:
                float rightDistance = Vector3.Dot(transform.right, _colregDetector.TargetPosition - transform.position);
                offset = (Mathf.Sign(-rightDistance) * transform.right + transform.forward) * (_obstacleAvoidanceDistance + _colregDetector.TargetRadius);
                _waypointAdjuster = (waypoint) => _colregDetector.TargetPosition + offset;
                _waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            case ColregDetector.ColregState.CrossingFromRight:
                offset = _colregDetector.TargetPosition;
                _waypointAdjuster = (waypoint) => offset;
                _waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            case ColregDetector.ColregState.CrossingFromLeft:
                _waypointAdjuster = (waypoint) => waypoint;
                _waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            default:
                _waypointAdjuster = (waypoint) => waypoint;
                _waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Util.UnitSquare(transform.forward.XZ()).XZ() * 5f);
        if (IsArrivedToDestination())
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_currentWaypoint, _targetDeadZoneRadius);
        // Gizmos.color = new Color(transform.GetInstanceID() % 255 / 255f, (transform.GetInstanceID() * 1453f) % 255 / 255f, (transform.GetInstanceID() * 571f) % 255 / 255f);
        // for (int i = 0; i < _waypoints.Length; i++)
        // {
        //     Gizmos.DrawSphere(_waypoints[i], _targetDeadZoneRadius * 0.5f);
        // }
    }

    private bool ShouldWaitForPassage()
    {
        if (_colregDetector.State == ColregDetector.ColregState.None)
        {
            return false;
        }

        bool isPathClear = GridManager.Instance.IsClearPath(transform.position, _currentWaypoint);

        return !isPathClear;
    }

    private enum TargetOvershootOptions { DoNothing, Skip }
}
