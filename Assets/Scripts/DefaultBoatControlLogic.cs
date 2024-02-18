using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Path;
using UNV.Pathfinding;

public class DefaultBoatControlLogic : BoatControlLogic
{
    private Vector3[] waypoints;
    private int currentWaypointIndex, previousWaypointIndex;

    private ColregDetector colregDetector;

    [SerializeField] private DefaultBoatControlLogicParametersScriptableObject parameters;

    private float colregDetectionRadius;
    private float colregAvoidanceRadius;
    private float colregCooldown;

    private Vector3 boatPosition
    {
        get
        {
            GetBoatPositionAndRotation(out Vector3 position, out _);
            return position;
        }
    }

    private Vector3 currentWaypoint => waypoints != null && waypoints.Length > 0 ? waypointAdjuster(waypoints[currentWaypointIndex]) : boatPosition;
    private Vector3 previousWaypoint => waypoints != null && waypoints.Length > 0 ? waypoints[previousWaypointIndex] : boatPosition;

    public Vector3 CurrentWaypoint => currentWaypoint;

    private Func<Vector3, Vector3> waypointAdjuster = (waypoint) => waypoint;
    private Func<Vector3, Vector3> waypointDirectionAdjuster = (waypointDirection) => waypointDirection;

    public DefaultBoatControlLogic() : base()
    {
    }

    public override void FinalizeInitialization()
    {
        SetUpColregDetector();
    }

    public void SetUpColregDetector()
    {
        colregDetector = new ColregDetector
        {
            GetBoatPosition = () => boatPosition,
            GetBoatForward = () => GetBoatForward(),
            GetBoatRight = () => GetBoatRight(),
            GetBoatUp = () => GetBoatUp(),
            boatId = BoatId,
            movingObstaclePositionEstimator = movingObstaclePositionEstimator,
            detectionRadius = colregDetectionRadius,
            avoidanceRadius = colregAvoidanceRadius,
            cooldown = colregCooldown
        };
        colregDetector.OnColregStateChange += OnColregStateChange;
    }

    public override void SetParameters(BoatControlLogicParametersScriptableObject parameters)
    {
        this.parameters = parameters as DefaultBoatControlLogicParametersScriptableObject;
        colregDetectionRadius = this.parameters.colregDetectionRadius;
        colregAvoidanceRadius = this.parameters.colregAvoidanceRadius;
        colregCooldown = this.parameters.colregCooldown;
    }

    public override void SetDestination(Vector3 destination)
    {
        base.SetDestination(destination);
        tilingManager.GenerateTiling();
        DilateObstacles();
        RequestPath();
    }

    private void DilateObstacles()
    {
        if (!tilingManager.DilatedBefore)
        {
            tilingManager.DilateObstacles(Mathf.FloorToInt(boatInformation.ObstacleAvoidanceDistance / tilingManager.NodeSize));
        }
    }

    private void RequestPath()
    {
        pathRequestManager.RequestPath(
            boatPosition,
            Destination,
            OnPathFound,
            parameters.steerCoefficient * boatInformation.SteerPower / boatInformation.MaxSpeed,
            new AStarWithRotation.GetDefaultDirectionDelegate(() => Util.UnitSquare(GetBoatForward().XZ()))
        );
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        waypoints = success ? (parameters.useBezierPath ? PathProcessing.GetBezierPath(path, parameters.bezierPathSampleCount) : path) : null;
        currentWaypointIndex = 0;
        OnPathFoundHook?.Invoke(waypoints, success);
    }

    private bool IsTargetReached()
    {
        float distanceToTarget = (currentWaypoint - boatPosition).magnitude;
        return distanceToTarget <= boatInformation.TargetDeadZoneRadius;
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
                (Vector3.Dot(GetBoatForward(), waypoints[currentWaypointIndex] - boatPosition) < 0) &&
                ((waypoints[currentWaypointIndex] - boatPosition).magnitude < colregDetector.DetectionRadius)
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
        if (Vector3.Distance(boatPosition, currentWaypoint) > boatInformation.ObstacleAvoidanceDistance * 1.5f)
        {
            return false;
        }
        Vector2 targetDirection = (currentWaypoint - boatPosition).XZ().normalized;
        Vector2 forward = GetBoatForward().XZ().normalized;
        float targetAngle = Vector2.Angle(targetDirection, forward);
        return targetAngle > 90f;
    }

    public void OnTargetOvershoot()
    {
        if (parameters.onTargetOvershoot == TargetOvershootOptions.Skip)
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

    public override float GetThrust()
    {
        colregDetector.Update();

        if (waypoints == null || waypoints.Length == 0)
        {
            return 0;
        }

        if (ShouldWaitForPassage())
        {
            return 0;
        }

        if (IsTargetReached())
        {
            OnTargetReached();
        }

        if (IsArrivedToDestination())
        {
            return 0;
        }

        if (IsTargetOvershoot())
        {
            OnTargetOvershoot();
        }

        return 1f;
    }

    public override float GetRudderAngle()
    {
        colregDetector.Update();

        if (waypoints == null || waypoints.Length == 0)
        {
            return 0;
        }

        if (ShouldWaitForPassage())
        {
            return 0;
        }

        if (IsTargetReached())
        {
            OnTargetReached();
        }

        if (IsArrivedToDestination())
        {
            return 0;
        }

        if (IsTargetOvershoot())
        {
            OnTargetOvershoot();
        }

        Vector3 waypointDirection = currentWaypoint - previousWaypoint;

        if (currentWaypointIndex == 0)
        {
            int nextWaypointIndex = currentWaypointIndex + 1;
            Vector3 nextWaypoint = nextWaypointIndex < waypoints.Length ? waypoints[nextWaypointIndex] : currentWaypoint + (currentWaypoint - previousWaypoint);
            waypointDirection = nextWaypoint - currentWaypoint;
        }

        float rudderAngle = FuzzyGuidanceController.GetCrispRudderAngle(
            boatPosition,
            currentWaypoint,
            GetBoatForward(),
            waypointDirectionAdjuster(waypointDirection)
        );

        return rudderAngle;
    }

    private void OnColregStateChange(ColregDetector.ColregState state)
    {
        Vector3 offset;
        switch (state)
        {
            case ColregDetector.ColregState.HeadOn:
                offset = (colregDetector.TargetPosition + boatPosition) / 2 + GetBoatRight() * (boatInformation.ObstacleAvoidanceDistance + colregDetector.TargetRadius);
                waypointAdjuster = (waypoint) => offset;
                waypointDirectionAdjuster = (waypointDirection) => waypointDirection;
                break;
            case ColregDetector.ColregState.Overtaking:
                float rightDistance = Vector3.Dot(GetBoatRight(), colregDetector.TargetPosition - boatPosition);
                offset = (Mathf.Sign(-rightDistance) * GetBoatRight() + GetBoatForward()) * (boatInformation.ObstacleAvoidanceDistance + colregDetector.TargetRadius);
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

    private bool ShouldWaitForPassage()
    {
        if (colregDetector.State == ColregDetector.ColregState.None)
        {
            return false;
        }

        bool isPathClear = tilingManager.IsClearPath(boatPosition, currentWaypoint);

        return !isPathClear;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawLine(boatPosition, boatPosition + Util.UnitSquare(GetBoatForward().XZ()).XZ() * 5f);
        if (IsArrivedToDestination())
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(currentWaypoint, boatInformation.TargetDeadZoneRadius);

        if (parameters.showColregDetectorGizmos)
        {
            colregDetector.OnDrawGizmos();
        }
    }

    public enum TargetOvershootOptions { DoNothing, Skip }
}
