using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FuzzyGuidanceControllerTest : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _nextTarget;
    [SerializeField] private float _targetDeadZoneRadius;
    [SerializeField] private float _forwardSpeed;
    [SerializeField] private float _angularSpeedScale;
    [SerializeField] private float _rudderAngle;

    private Vector3 _targetHeading => (_nextTarget.position - _target.position).normalized;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distance = (_target.position - transform.position).magnitude;
        if (distance < _targetDeadZoneRadius)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            return;
        }
        _rudderAngle = FuzzyGuidanceController.GetCrispRudderAngle(transform.position, _target.position, transform.forward, _targetHeading);
        _rigidbody.velocity = transform.forward * _forwardSpeed;
        _rigidbody.angularVelocity = Vector3.up * _rudderAngle * _forwardSpeed * _angularSpeedScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_target.position, _targetDeadZoneRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_target.position, _target.position + _targetHeading * 5f);
    }
}
