using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
[RequireComponent(typeof(BoatController))]
public class ColregDetector : MonoBehaviour
{
    [SerializeField] private MovingObstaclePositionEstimator _movingObstaclePositionEstimator;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _avoidanceRadius;
    [SerializeField] private float _cooldown = 0.1f;

    public float DetectionRadius => _detectionRadius;
=======
public class ColregDetector
{
    public MovingObstaclePositionEstimator movingObstaclePositionEstimator;
    public float detectionRadius;
    public float avoidanceRadius;
    public float cooldown = 0.1f;

    public int boatId;

    public GetVector3Delegate GetBoatPosition, GetBoatForward, GetBoatRight, GetBoatUp;
    public delegate Vector3 GetVector3Delegate();

    public float DetectionRadius => detectionRadius;
>>>>>>> Stashed changes

    public delegate void ColregStateChangeDelegate(ColregState state);
    public ColregStateChangeDelegate OnColregStateChange;

<<<<<<< Updated upstream
    public ColregState State => _state;
    private ColregState _state = ColregState.None;
=======
    public ColregState State => state;
    private ColregState state = ColregState.None;
>>>>>>> Stashed changes

    public Vector3 TargetPosition { get; private set; }
    public Vector3 TargetVelocity { get; private set; }
    public float TargetRadius { get; private set; }

<<<<<<< Updated upstream
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
=======
    private float cooldownTimer = 0f;

    public void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            UpdateColregState();
            cooldownTimer = cooldown;
        }
    }

    public void OnDrawGizmos()
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
=======
        Gizmos.DrawWireSphere(GetBoatPosition(), detectionRadius);
>>>>>>> Stashed changes
    }

    private void UpdateColregState()
    {
        int movingObstacleCount = 0;
<<<<<<< Updated upstream
        ColregState state = _state;
        foreach (var movingObstacle in _movingObstaclePositionEstimator.GetEstimatedWithout(transform))
=======
        ColregState state = this.state;
        foreach (var movingObstacle in movingObstaclePositionEstimator.GetEstimatedWithout(boatId))
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        if (_state == state)
=======
        if (this.state == state)
>>>>>>> Stashed changes
        {
            return;
        }

<<<<<<< Updated upstream
        _state = state;
        OnColregStateChange?.Invoke(_state);
=======
        this.state = state;
        OnColregStateChange?.Invoke(this.state);
>>>>>>> Stashed changes
    }

    private bool CanChangeStateTo(ColregState state)
    {
<<<<<<< Updated upstream
        if (_state == ColregState.HeadOn)
        {
            return false;
        }
        if (_state == ColregState.CrossingFromRight && state == ColregState.HeadOn)
=======
        if (this.state == ColregState.HeadOn)
        {
            return false;
        }
        if (this.state == ColregState.CrossingFromRight && state == ColregState.HeadOn)
>>>>>>> Stashed changes
        {
            return false;
        }
        return true;
    }

    private bool IsInDetectionRadius(Vector3 position)
    {
<<<<<<< Updated upstream
        return Vector3.Distance(transform.position, position) <= _detectionRadius;
=======
        return Vector3.Distance(GetBoatPosition(), position) <= detectionRadius;
>>>>>>> Stashed changes
    }

    private bool IsHeadOn(Vector3 targetPosition, Vector3 targetVelocity, float targetRadius)
    {
<<<<<<< Updated upstream
        Vector3 difference = targetPosition - transform.position;

        float angleRange = 30f;
        float angle = Vector2.Angle(transform.forward.XZ(), difference.XZ());
=======
        Vector3 difference = targetPosition - GetBoatPosition();

        float angleRange = 30f;
        float angle = Vector2.Angle(GetBoatForward().XZ(), difference.XZ());
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        float forwardDistance = Vector3.Dot(transform.forward, targetPosition - transform.position);
=======
        float forwardDistance = Vector3.Dot(GetBoatForward(), targetPosition - GetBoatPosition());
>>>>>>> Stashed changes

        if (forwardDistance < 0)
        {
            return false;
        }

<<<<<<< Updated upstream
        float rightAbsoluteDistance = Mathf.Abs(Vector3.Dot(transform.right, targetPosition - transform.position));

        if (rightAbsoluteDistance > targetRadius + _avoidanceRadius)
=======
        float rightAbsoluteDistance = Mathf.Abs(Vector3.Dot(GetBoatRight(), targetPosition - GetBoatPosition()));

        if (rightAbsoluteDistance > targetRadius + avoidanceRadius)
>>>>>>> Stashed changes
        {
            return false;
        }

        float overtakeAngleRange = 60f;
<<<<<<< Updated upstream
        float relativeAngle = Vector2.Angle(-targetVelocity.XZ(), (transform.position - targetPosition).XZ());
=======
        float relativeAngle = Vector2.Angle(-targetVelocity.XZ(), (GetBoatPosition() - targetPosition).XZ());
>>>>>>> Stashed changes

        if (relativeAngle > overtakeAngleRange)
        {
            return false;
        }

<<<<<<< Updated upstream
        Vector3 difference = targetPosition - transform.position;

        float forwardAngleRange = 30f;
        float angle = Vector2.Angle(transform.forward.XZ(), difference.XZ());
=======
        Vector3 difference = targetPosition - GetBoatPosition();

        float forwardAngleRange = 30f;
        float angle = Vector2.Angle(GetBoatForward().XZ(), difference.XZ());
>>>>>>> Stashed changes

        if (angle > forwardAngleRange)
        {
            return false;
        }

        return true;
    }

    private bool IsCrossingFromRight(Vector3 targetPosition, Vector3 targetVelocity)
    {
<<<<<<< Updated upstream
        float forwardDistance = Vector3.Dot(transform.forward, targetPosition - transform.position);
=======
        float forwardDistance = Vector3.Dot(GetBoatForward(), targetPosition - GetBoatPosition());
>>>>>>> Stashed changes

        if (forwardDistance < 0)
        {
            return false;
        }

<<<<<<< Updated upstream
        float rightDistance = Vector3.Dot(transform.right, targetPosition - transform.position);
=======
        float rightDistance = Vector3.Dot(GetBoatRight(), targetPosition - GetBoatPosition());
>>>>>>> Stashed changes

        if (rightDistance < 0)
        {
            return false;
        }

<<<<<<< Updated upstream
        Vector3 myVelocity = _movingObstaclePositionEstimator.GetEstimated(transform).velocity;
        Vector3 relativeVelocity = targetVelocity - myVelocity;

        float relativeVelocityAngle = Vector2.Angle(transform.right.XZ(), -relativeVelocity.XZ());
        float relativePositionAngle = Vector2.Angle(transform.right.XZ(), (targetPosition - transform.position).XZ());
=======
        Vector3 myVelocity = movingObstaclePositionEstimator.GetEstimated(boatId).velocity;
        Vector3 relativeVelocity = targetVelocity - myVelocity;

        float relativeVelocityAngle = Vector2.Angle(GetBoatRight().XZ(), -relativeVelocity.XZ());
        float relativePositionAngle = Vector2.Angle(GetBoatRight().XZ(), (targetPosition - GetBoatPosition()).XZ());
>>>>>>> Stashed changes

        float angleThreshold = 10f;

        if (Mathf.Abs(relativeVelocityAngle - relativePositionAngle) > angleThreshold)
        {
            return false;
        }

        return true;
    }

    private bool IsCrossingFromLeft(Vector3 targetPosition, Vector3 targetVelocity)
    {
<<<<<<< Updated upstream
        float forwardDistance = Vector3.Dot(transform.forward, targetPosition - transform.position);
=======
        float forwardDistance = Vector3.Dot(GetBoatForward(), targetPosition - GetBoatPosition());
>>>>>>> Stashed changes

        if (forwardDistance < 0)
        {
            return false;
        }

<<<<<<< Updated upstream
        float rightDistance = Vector3.Dot(transform.right, targetPosition - transform.position);
=======
        float rightDistance = Vector3.Dot(GetBoatRight(), targetPosition - GetBoatPosition());
>>>>>>> Stashed changes

        if (rightDistance > 0)
        {
            return false;
        }

<<<<<<< Updated upstream
        Vector3 myVelocity = _movingObstaclePositionEstimator.GetEstimated(transform).velocity;
        Vector3 relativeVelocity = targetVelocity - myVelocity;

        float relativeVelocityAngle = Vector2.Angle(transform.right.XZ(), -relativeVelocity.XZ());
        float relativePositionAngle = Vector2.Angle(transform.right.XZ(), (targetPosition - transform.position).XZ());
=======
        Vector3 myVelocity = movingObstaclePositionEstimator.GetEstimated(boatId).velocity;
        Vector3 relativeVelocity = targetVelocity - myVelocity;

        float relativeVelocityAngle = Vector2.Angle(GetBoatRight().XZ(), -relativeVelocity.XZ());
        float relativePositionAngle = Vector2.Angle(GetBoatRight().XZ(), (targetPosition - GetBoatPosition()).XZ());
>>>>>>> Stashed changes

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
