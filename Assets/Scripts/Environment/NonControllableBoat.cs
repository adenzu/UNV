using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NonControllableBoat : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _angularSpeedScale = 0.01f;
    [SerializeField] private float _rudderAngleRange = 30f;
    [SerializeField] private float _rudderAngleChangeInterval = 10f;

    private float _rudderAngle = 0f;
    private float _targetRudderAngle = 0f;
    private float _timer = 0f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_timer >= _rudderAngleChangeInterval)
        {
            _targetRudderAngle = Random.Range(-_rudderAngleRange, _rudderAngleRange);
            _timer = 0;
        }
        _rigidbody.velocity = transform.forward * _speed;
        _rigidbody.angularVelocity = Vector3.up * _speed * _angularSpeedScale * Mathf.Lerp(_rudderAngle, _targetRudderAngle, _timer / _rudderAngleChangeInterval);
    }
}
