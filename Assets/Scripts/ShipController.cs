using UnityEngine;
using UNV.Path;
using UNV.Pathfinding;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField] private float _obstacleAvoidanceDistance = 5f;
    [SerializeField] private float _targetDeadZoneRadius = 0.1f;

    [SerializeField] private bool _useBezierPath = true;
    [SerializeField] private int _bezierPathNumSamples = 10;

    [SerializeField] private Transform _motor;

    [SerializeField] private float _steerPower = 500f;
    [SerializeField] private float _power = 5f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _angularSpeedScale = 0.01f;

    [SerializeField] private float _steerCoefficient = 1f;

    [SerializeField] private TargetOvershootOptions _onTargetOvershoot = TargetOvershootOptions.DoNothing;

    public float ShipRadius { get; private set; }
    public float TurnMinRadius => ShipRadius * _maxSpeed / _steerPower;

    private float _thrust;
    private float _rudderAngle;

    private Vector3 _destination;
    private Vector3[] _waypoints;
    private int _currentWaypointIndex, _previousWaypointIndex;
    private Vector3 _currentWaypoint => _waypoints != null && _waypoints.Length > 0 ? _waypoints[_currentWaypointIndex] : transform.position;
    private Vector3 _previousWaypoint => _waypoints != null && _waypoints.Length > 0 ? _waypoints[_previousWaypointIndex] : transform.position;

    private Rigidbody _rigidbody;
    private GridManager _gridManager;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gridManager = GridManager.Instance;
        ShipRadius = (transform.position - _motor.position).XZ().magnitude;
    }

    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
        DilateObstacles();
        RequestPath();
    }

    private void DilateObstacles()
    {
        if (!_gridManager.DilatedBefore)
        {
            _gridManager.DilateObstacles(Mathf.FloorToInt(_obstacleAvoidanceDistance / _gridManager.NodeSize));
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
        _previousWaypointIndex = _currentWaypointIndex;
        _currentWaypointIndex++;
        if (IsArrivedToDestination())
        {
            OnArrivalToDestination();
        }
    }

    private bool IsTargetOvershoot()
    {
        if (_waypoints == null || _currentWaypointIndex >= _waypoints.Length - 1)
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
    }

    private void FixedUpdate()
    {
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
            waypointDirection
        );

        Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward).normalized;

        Vector3 desiredForwardVelocity = forward * _maxSpeed * _thrust;
        float desiredAngularSpeed = _rudderAngle * _maxSpeed * _thrust * _angularSpeedScale;

        PhysicsHelper.ApplyAccelerationToReachAngularSpeed(_rigidbody, desiredAngularSpeed, _steerPower);
        PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, desiredForwardVelocity, _power);
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
        Gizmos.color = Color.red;
        foreach (Vector3 waypoint in _waypoints)
        {
            Gizmos.DrawSphere(waypoint, 1);
        }
    }

    private enum TargetOvershootOptions { DoNothing, Skip }
}
