using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class RandomDrifter : MonoBehaviour
{
    [SerializeField, Min(0)] private float _force = 1f;
    [SerializeField, Min(0)] private float _speed = 1f;
    [SerializeField, Min(0)] private float _duration = 1f;
    [SerializeField, Min(0)] private float _cooldown = 1f;

    private float _durationTimer;
    private float _cooldownTimer;

    [SerializeField] private Vector3Bool _driftAxis = new Vector3Bool { x = true, y = false, z = true };

    private Rigidbody _rigidbody;
    private Vector3 _direction;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.fixedDeltaTime;
            return;
        }

        if (_durationTimer > 0)
        {
            _durationTimer -= Time.fixedDeltaTime;
            PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, _direction * _speed, _force);
        }
        else
        {
            RandomDirection();
            _durationTimer = _duration;
            _cooldownTimer = _cooldown;
        }
    }

    private void RandomDirection()
    {
        _direction = Vector3.Scale(Random.insideUnitSphere, new Vector3(_driftAxis.x ? 1 : 0, _driftAxis.y ? 1 : 0, _driftAxis.z ? 1 : 0)).normalized;
    }

    [Serializable]
    private struct Vector3Bool
    {
        public bool x;
        public bool y;
        public bool z;
    }
}
