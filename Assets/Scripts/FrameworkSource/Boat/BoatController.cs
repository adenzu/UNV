using System;
using UnityEditor;
using UnityEngine;
using UNV.Path;
using UNV.Pathfinding;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [SerializeField] private TilingManager tilingManager;
    [SerializeField] private PathRequestManager pathRequestManager;
    [SerializeField] private MovingObstaclePositionEstimator movingObstaclePositionEstimator;

    [SerializeField] private MonoScript boatControlLogicScript;

    [SerializeField] private BoatInformationScriptableObject boatInformation;
    [SerializeField] private BoatControlLogicParametersScriptableObject boatControlLogicParameters;

    public delegate void OnArrivalToDestinationDelegate();
    public event OnArrivalToDestinationDelegate OnArrivalToDestinationHook;

    public event OnDrawGizmosDelegate OnDrawGizmosHook;
    public delegate void OnDrawGizmosDelegate();

    private BoatControlLogic boatControlLogic;

    private float thrust;
    private float rudderAngle;
    private Vector3 destination;

    private Rigidbody rb;

    private void OnValidate()
    {
        if (!Util.HandleMonoScriptFieldClassInheritance(boatControlLogicScript, typeof(BoatControlLogic), "Boat Control Logic"))
        {
            boatControlLogicScript = null;
        }
    }

    private void Awake()
    {
        if (boatControlLogicScript != null)
        {
            boatControlLogic = Util.InstantiateMonoScriptObject<BoatControlLogic>(boatControlLogicScript);
            boatControlLogic.SetParameters(boatControlLogicParameters);
            boatControlLogic.pathRequestManager = pathRequestManager;
            boatControlLogic.tilingManager = tilingManager;
            boatControlLogic.boatInformation = boatInformation;
            boatControlLogic.movingObstaclePositionEstimator = movingObstaclePositionEstimator;
            boatControlLogic.GetBoatPositionAndRotation = transform.GetPositionAndRotation;
            boatControlLogic.GetBoatForward = () => transform.forward;
            boatControlLogic.GetBoatRight = () => transform.right;
            boatControlLogic.GetBoatUp = () => transform.up;
            boatControlLogic.OnPathFoundHook += OnPathFound;
            boatControlLogic.OnArrivalToDestinationHook += () => OnArrivalToDestinationHook?.Invoke();
            boatControlLogic.SetBoatController(this);
            boatControlLogic.FinalizeInitialization();
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDestination(Vector3 destination)
    {
        boatControlLogic.SetDestination(destination);
    }

    private void RequestPath()
    {
        SetDestination(destination);
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        if (!success)
        {
            Debug.Log("Path not found");
            Invoke(nameof(RequestPath), 1f);
        }
    }

    private void FixedUpdate()
    {

        thrust = boatControlLogic.GetThrust();
        rudderAngle = boatControlLogic.GetRudderAngle();

        rudderAngle = Mathf.Clamp(rudderAngle, -boatInformation.MaxRudderAngle, boatInformation.MaxRudderAngle);

        Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward).normalized;

        Vector3 desiredForwardVelocity = forward * boatInformation.MaxSpeed * thrust;
        float desiredAngularSpeed = rudderAngle * boatInformation.MaxSpeed * thrust * boatInformation.AngularSpeedScale;

        PhysicsHelper.ApplyAccelerationToReachAngularSpeed(rb, desiredAngularSpeed, boatInformation.SteerPower);
        PhysicsHelper.ApplyForceToReachVelocity(rb, desiredForwardVelocity, boatInformation.Power);
    }

    private void OnDrawGizmos()
    {
        OnDrawGizmosHook?.Invoke();
    }
}
