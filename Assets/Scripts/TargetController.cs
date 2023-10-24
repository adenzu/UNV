using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private float _cooldown = 5f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _distance = 10f;

    private float _timer = 0f;
    private Vector3 _targetPosition;

    private void Start()
    {
        _timer = _cooldown;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _timer = _cooldown;
            _targetPosition = (Vector3)Random.insideUnitCircle * _distance + transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
    }
}
