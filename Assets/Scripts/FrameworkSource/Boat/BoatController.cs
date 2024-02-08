using System;
using UnityEngine;
using UNV.Path;
using UNV.Pathfinding;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathRequestManager pathRequestManager;

    [SerializeField] private float obstacleAvoidanceDistance = 5f;
    [SerializeField] private float targetDeadZoneRadius = 0.1f;

    [SerializeField] private bool useBezierPath = true;
    [SerializeField] private int bezierPathSampleCount = 10;

    [SerializeField] private float steerPower = 500f;
    [SerializeField] private float power = 5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float angularSpeedScale = 0.01f;

    [SerializeField] private float steerCoefficient = 1f;

    [SerializeField] private TargetOvershootOptions onTargetOvershoot = TargetOvershootOptions.DoNothing;

    [SerializeField] private ColregDetector colregDetector;

    public delegate void OnArrivalToDestinationDelegate();
    public OnArrivalToDestinationDelegate OnArrivalToDestinationHook;

    private float thrust;
    private float rudderAngle;

    private Vector3 destination;
    private Vector3[] waypoints;
    private int currentWaypointIndex, previousWaypointIndex;
    private Vector3 currentWaypoint => waypoints != null && waypoints.Length > 0 ? waypointAdjuster(waypoints[currentWaypointIndex]) : transform.position;
    private Vector3 previousWaypoint => waypoints != null && waypoints.Length > 0 ? waypoints[previousWaypointIndex] : transform.position;

    public Vector3 CurrentWaypoint => currentWaypoint;

    private Func<Vector3, Vector3> waypointAdjuster = (waypoint) => waypoint;
    private Func<Vector3, Vector3> waypointDirectionAdjuster = (waypointDirection) => waypointDirection;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (colregDetector != null)
        {
            colregDetector.OnColregStateChange += OnColregStateChange;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        gridManager.GenerateGrid();
        DilateObstacles();
        RequestPath();
    }

    private void DilateObstacles()
    {
        if (!gridManager.DilatedBefore)
        {
            gridManager.DilateObstacles(Mathf.FloorToInt(obstacleAvoidanceDistance / gridManager.NodeSize));
        }
    }

    private void RequestPath()
    {
        pathRequestManager.RequestPath(
            transform.position,
            destination,
            steerCoefficient * steerPower / maxSpeed,
            OnPathFound,
            () => Util.UnitSquare(transform.forward.XZ())
        );
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        if (!success)
        {
            gridManager.GenerateGrid();
            DilateObstacles();
            Invoke(nameof(RequestPath), 1f);
        }
        waypoints = success ? (useBezierPath ? PathProcessing.GetBezierPath(path, bezierPathSampleCount) : path) : null;
        currentWaypointIndex = 0;
    }

    private bool IsTargetReached()
    {
        float distanceToTarget = (currentWaypoint - transform.position).magnitude;
        return distanceToTarget <= targetDeadZoneRadius;
    }

    private void OnTargetReached()
    {
        if
        (
            colregDetector.State == ColregDetector.ColregState.None ||
            colregDetector.State == ColregDetector.ColregState.CrossingFromLeft
        )
        {
            previousWaypointIndex = currentWaypointIndex;
            currentWaypointIndex++;
        }
        else
        {
            while
            (
                (currentWaypointIndex < waypoints.Length - 1) &&
                (Vector3.Dot(transform.forward, waypoints[currentWaypointIndex] - transform.position) < 0) &&
                ((waypoints[currentWaypointIndex] - transform.position).magnitude < colregDetector.DetectionRadius)
            )
            {
                previousWaypointIndex = currentWaypointIndex;
                currentWaypointIndex++;
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
        if (Vector3.Distance(transform.position, currentWaypoint) > obstacleAvoidanceDistance * 1.5f)
        {
            return false;
        }
        Vector2 targetDirection = (currentWaypoint - transform.position).XZ().normalized;
        Vector2 forward = transform.forward.XZ().normalized;
        float targetAngle = Vector2.Angle(targetDirection, forward);
        return targetAngle > 90f;
    }

    public void OnTargetOvershoot()
    {
        if (onTargetOvershoot == TargetOvershootOptions.Skip)
        {
            OnTargetReached();
        }
    }

    private bool IsArrivedToDestination()
    {
        return waypoints == null || currentWaypointIndex >= waypoints.Length;
    }

    private void OnArrivalToDestination()
    {
        waypoints = null;
        currentWaypointIndex = 0;
        previousWaypointIndex = 0;
        OnArrivalToDestinationHook?.Invoke();
    }

    private void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        if (ShouldWaitForPassage())
        {
            return;
        }

        Vector3 waypointDirection = currentWaypoint - previousWaypoint;

        if (currentWaypointIndex == 0)
        {
            int nextWaypointIndex = currentWaypointIndex + 1;
            Vector3 nextWaypoint = nextWaypointIndex < waypoints.Length ? waypoints[nextWaypointIndex] : currentWaypoint + (currentWaypoint - previousWaypoint);
            waypointDirection = nextWaypoint - currentWaypoint;
        }

        thrust = 1f;
        rudderAngle = FuzzyGuidanceController.GetCrispRudderAngle(
            transform.position,
            currentWaypoint,
            transform.forward,
            waypointDirectionAdjuster(waypointDirection)
        );

        Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward).normalized;

        Vector3 desiredForwardVelocity = forward * maxSpeed * thrust;
        float desiredAngularSpeed = rudderAngle * maxSpeed * thrust * angularSpeedScale;

        PhysicsHelper.ApplyAccelerationToReachAngularSpeed(_rigidbody, desiredAngularSpeed, steerPower);
        PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, desiredForwardVelocity, power);

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
                offset = (colregDetector.TargetPosition + transform.position) / 2 + transform.right * (obstacleAvoidanceDistance + colregDetector.TargetRadius);
                waypointAdjuster = (waypoint) => offset;
                waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            case ColregDetector.ColregState.Overtaking:
                float rightDistance = Vector3.Dot(transform.right, colregDetector.TargetPosition - transform.position);
                offset = (Mathf.Sign(-rightDistance) * transform.right + transform.forward) * (obstacleAvoidanceDistance + colregDetector.TargetRadius);
                waypointAdjuster = (waypoint) => colregDetector.TargetPosition + offset;
                waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            case ColregDetector.ColregState.CrossingFromRight:
                offset = colregDetector.TargetPosition;
                waypointAdjuster = (waypoint) => offset;
                waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            case ColregDetector.ColregState.CrossingFromLeft:
                waypointAdjuster = (waypoint) => waypoint;
                waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            default:
                waypointAdjuster = (waypoint) => waypoint;
                waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
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
        Gizmos.DrawSphere(currentWaypoint, targetDeadZoneRadius);
        // Gizmos.color = new Color(transform.GetInstanceID() % 255 / 255f, (transform.GetInstanceID() * 1453f) % 255 / 255f, (transform.GetInstanceID() * 571f) % 255 / 255f);
        // for (int i = 0; i < _waypoints.Length; i++)
        // {
        //     Gizmos.DrawSphere(_waypoints[i], _targetDeadZoneRadius * 0.5f);
        // }
    }

    private bool ShouldWaitForPassage()
    {
        if (colregDetector.State == ColregDetector.ColregState.None)
        {
            return false;
        }

        bool isPathClear = gridManager.IsClearPath(transform.position, currentWaypoint);

        return !isPathClear;
    }

    private enum TargetOvershootOptions { DoNothing, Skip }
}
