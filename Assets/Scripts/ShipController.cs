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

    private float _thrust;
    private float _steer;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        PlaceholderAI.Inputs inputs = new PlaceholderAI.Inputs
        {
            thrust = _thrust,
            steer = _steer,
            right = transform.right,
            position = transform.position,
            target = _target.position,
            targetDeadZone = _targetDeadZone
        };

        PlaceholderAI.Outputs outputs = PlaceholderAI.Predict(inputs);

        _thrust = outputs.thrust; //+= outputs.thrustJerk * Time.fixedDeltaTime;
        _steer = outputs.steer; //+= outputs.steerJerk * Time.fixedDeltaTime;

        _thrust = Mathf.Clamp(_thrust, -1f, 1f);
        _steer = Mathf.Clamp(_steer, -1f, 1f);

        Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

        _rigidbody.AddForceAtPosition(-_steer * transform.right * _steerPower, _motor.position);
        PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, forward * _maxSpeed * _thrust, _power);
    }
}
