using UnityEngine;
using UNV.Pathfinding;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField] private float _obstacleAvoidanceDistance = 5f;
    [SerializeField] private float _targetDeadZone = 0.1f;

    [SerializeField] private Transform _motor;

    [SerializeField] private float _steerPower = 500f;
    [SerializeField] private float _power = 5f;
    [SerializeField] private float _maxSpeed = 10f;

    [SerializeField] private float _steerCoefficient = 1f;

    [SerializeField] private TargetOvershootOptions _onTargetOvershoot = TargetOvershootOptions.DoNothing;

    public float ShipRadius { get; private set; }
    public float TurnMinRadius => ShipRadius * _maxSpeed / _steerPower;

    private float _thrust;
    private float _steer;

    private Vector3 _destination;
    private Vector3[] _waypoints;
    private int _currentWaypointIndex;
    private Vector3 _currentWaypoint => _waypoints != null && _waypoints.Length > 0 ? _waypoints[_currentWaypointIndex] : transform.position;

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
            _steerCoefficient,
            OnPathFound,
            () => Util.UnitSquare(transform.forward.XZ())
        );
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        _waypoints = success ? path : null;
        _currentWaypointIndex = 0;
    }

    private bool IsTargetReached()
    {
        return (_currentWaypoint - transform.position).magnitude < _targetDeadZone;
    }

    private void OnTargetReached()
    {
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
    }

    private void FixedUpdate()
    {
        if (IsArrivedToDestination())
        {
            return;
        }

        if (IsTargetReached())
        {
            OnTargetReached();
        }

        if (IsTargetOvershoot())
        {
            OnTargetOvershoot();
        }

        PlaceholderAI.Inputs inputs = new PlaceholderAI.Inputs
        {
            thrust = _thrust,
            steer = _steer,
            right = transform.right,
            position = transform.position,
            target = _currentWaypoint,
            targetDeadZone = _targetDeadZone
        };

        PlaceholderAI.Outputs outputs = PlaceholderAI.Predict(inputs);

        _thrust = outputs.thrust; //+= outputs.thrustJerk * Time.fixedDeltaTime;
        _steer = outputs.steer; //+= outputs.steerJerk * Time.fixedDeltaTime;

        _thrust = Mathf.Clamp(_thrust, -1f, 1f);
        _steer = Mathf.Clamp(_steer, -1f, 1f);

        Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

        Vector3 thrustForce = forward * _maxSpeed * _thrust;
        Vector3 steerForce = -_steer * transform.right * _steerPower;

        _rigidbody.AddForceAtPosition(steerForce, _motor.position);
        PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, thrustForce, _power);
    }

    private void OnDrawGizmos()
    {
        if (IsArrivedToDestination())
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_currentWaypoint, _targetDeadZone);
        foreach (Vector3 waypoint in _waypoints)
        {
            Gizmos.DrawCube(waypoint, _gridManager.NodeSize * Vector3.one);
        }
    }

    private enum TargetOvershootOptions { DoNothing, Skip }
}
