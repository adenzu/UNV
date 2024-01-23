using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoatController))]
public class ColregDetector : MonoBehaviour
{
    [SerializeField] private MovingObstaclePositionEstimator _movingObstaclePositionEstimator;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _avoidanceRadius;
    [SerializeField] private float _cooldown = 0.1f;

    public float DetectionRadius => _detectionRadius;

    public delegate void ColregStateChangeDelegate(ColregState state);
    public ColregStateChangeDelegate OnColregStateChange;

    public ColregState State => _state;
    private ColregState _state = ColregState.None;

    public Vector3 TargetPosition { get; private set; }
    public Vector3 TargetVelocity { get; private set; }
    public float TargetRadius { get; private set; }

    private float _cooldownTimer = 0f;

    private BoatController _boatController;

    private void Start()
    {
        _boatController = GetComponent<BoatController>();
        if (_movingObstaclePositionEstimator == null)
        {
            _movingObstaclePositionEstimator = FindObjectOfType<MovingObstaclePositionEstimator>();
        }
    }

    private void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0f)
        {
            UpdateColregState();
            _cooldownTimer = _cooldown;
        }
    }

    private void OnDrawGizmos()
    {
        switch (State)
        {
            case ColregState.None:
                Gizmos.color = Color.white;
                break;
            case ColregState.HeadOn:
                Gizmos.color = Color.red;
                break;
            case ColregState.Overtaking:
                Gizmos.color = Color.yellow;
                break;
            case ColregState.CrossingFromRight:
                Gizmos.color = Color.green;
                break;
            case ColregState.CrossingFromLeft:
                Gizmos.color = Color.blue;
                break;
        }
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    private void UpdateColregState()
    {
        int movingObstacleCount = 0;
        ColregState state = _state;
        foreach (var movingObstacle in _movingObstaclePositionEstimator.GetEstimatedWithout(transform))
        {
            if
            (
                !IsInDetectionRadius(movingObstacle.position)
            )
            {
                continue;
            }

            movingObstacleCount++;
            TargetPosition = movingObstacle.position;
            TargetVelocity = movingObstacle.velocity;
            TargetRadius = movingObstacle.radius;

            if (CanChangeStateTo(ColregState.HeadOn) && IsHeadOn(TargetPosition, TargetVelocity, TargetRadius))
            {
                state = ColregState.HeadOn;
            }
            else if (CanChangeStateTo(ColregState.Overtaking) && IsOvertaking(TargetPosition, TargetVelocity, TargetRadius))
            {
                state = ColregState.Overtaking;
            }
            else if (CanChangeStateTo(ColregState.CrossingFromRight) && IsCrossingFromRight(TargetPosition, TargetVelocity))
            {
                state = ColregState.CrossingFromRight;
            }
            else if (CanChangeStateTo(ColregState.CrossingFromLeft) && IsCrossingFromLeft(TargetPosition, TargetVelocity))
            {
                state = ColregState.CrossingFromLeft;
            }
        }
        if (movingObstacleCount == 0)
        {
            state = ColregState.None;
        }
        SetState(state);
    }

    private void SetState(ColregState state)
    {
        if (_state == state)
        {
            return;
        }

        _state = state;
        OnColregStateChange?.Invoke(_state);
    }

    private bool CanChangeStateTo(ColregState state)
    {
        if (_state == ColregState.HeadOn)
        {
            return false;
        }
        if (_state == ColregState.CrossingFromRight && state == ColregState.HeadOn)
        {
            return false;
        }
        return true;
    }

    private bool IsInDetectionRadius(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= _detectionRadius;
    }

    private bool IsHeadOn(Vector3 targetPosition, Vector3 targetVelocity, float targetRadius)
    {
        Vector3 difference = targetPosition - transform.position;

        float angleRange = 30f;
        float angle = Vector2.Angle(transform.forward.XZ(), difference.XZ());

        if (angle > angleRange)
        {
            return false;
        }

        float otherAngle = Vector2.Angle(targetVelocity.XZ(), -difference.XZ());

        if (otherAngle > angleRange)
        {
            return false;
        }

        return true;
    }

    private bool IsOvertaking(Vector3 targetPosition, Vector3 targetVelocity, float targetRadius)
    {
        float forwardDistance = Vector3.Dot(transform.forward, targetPosition - transform.position);

        if (forwardDistance < 0)
        {
            return false;
        }

        float rightAbsoluteDistance = Mathf.Abs(Vector3.Dot(transform.right, targetPosition - transform.position));

        if (rightAbsoluteDistance > targetRadius + _avoidanceRadius)
        {
            return false;
        }

        float overtakeAngleRange = 60f;
        float relativeAngle = Vector2.Angle(-targetVelocity.XZ(), (transform.position - targetPosition).XZ());

        if (relativeAngle > overtakeAngleRange)
        {
            return false;
        }

        Vector3 difference = targetPosition - transform.position;

        float forwardAngleRange = 30f;
        float angle = Vector2.Angle(transform.forward.XZ(), difference.XZ());

        if (angle > forwardAngleRange)
        {
            return false;
        }

        return true;
    }

    private bool IsCrossingFromRight(Vector3 targetPosition, Vector3 targetVelocity)
    {
        float forwardDistance = Vector3.Dot(transform.forward, targetPosition - transform.position);

        if (forwardDistance < 0)
        {
            return false;
        }

        float rightDistance = Vector3.Dot(transform.right, targetPosition - transform.position);

        if (rightDistance < 0)
        {
            return false;
        }

        Vector3 myVelocity = _movingObstaclePositionEstimator.GetEstimated(transform).velocity;
        Vector3 relativeVelocity = targetVelocity - myVelocity;

        float relativeVelocityAngle = Vector2.Angle(transform.right.XZ(), -relativeVelocity.XZ());
        float relativePositionAngle = Vector2.Angle(transform.right.XZ(), (targetPosition - transform.position).XZ());

        float angleThreshold = 10f;

        if (Mathf.Abs(relativeVelocityAngle - relativePositionAngle) > angleThreshold)
        {
            return false;
        }

        return true;
    }

    private bool IsCrossingFromLeft(Vector3 targetPosition, Vector3 targetVelocity)
    {
        float forwardDistance = Vector3.Dot(transform.forward, targetPosition - transform.position);

        if (forwardDistance < 0)
        {
            return false;
        }

        float rightDistance = Vector3.Dot(transform.right, targetPosition - transform.position);

        if (rightDistance > 0)
        {
            return false;
        }

        Vector3 myVelocity = _movingObstaclePositionEstimator.GetEstimated(transform).velocity;
        Vector3 relativeVelocity = targetVelocity - myVelocity;

        float relativeVelocityAngle = Vector2.Angle(transform.right.XZ(), -relativeVelocity.XZ());
        float relativePositionAngle = Vector2.Angle(transform.right.XZ(), (targetPosition - transform.position).XZ());

        float angleThreshold = 10f;

        if (Mathf.Abs(relativeVelocityAngle - relativePositionAngle) > angleThreshold)
        {
            return false;
        }

        return true;
    }

    public enum ColregState
    {
        None,
        HeadOn,
        Overtaking,
        CrossingFromRight,
        CrossingFromLeft
    }
}
