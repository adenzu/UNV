using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _targetDeadZone = 0.1f;

    [SerializeField] private Transform _motor;

    [SerializeField] private float _steerPower = 500f;
    [SerializeField] private float _power = 5f;
    [SerializeField] private float _maxSpeed = 10f;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        PlaceholderAI.Inputs inputs = new PlaceholderAI.Inputs
        {
            right = transform.right,
            position = transform.position,
            target = _target.position,
            targetDeadZone = _targetDeadZone
        };

        PlaceholderAI.Outputs outputs = PlaceholderAI.Predict(inputs);

        float thrust = outputs.thrust;
        float steer = outputs.steer;

        Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

        _rigidbody.AddForceAtPosition(-steer * transform.right * _steerPower, _motor.position);
        PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, forward * _maxSpeed * thrust, _power);
    }
}
