using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // [SerializeField] private float _cooldown = 5f;
    // [SerializeField] private float _speed = 5f;
    // [SerializeField] private float _distance = 10f;

    [SerializeField] private bool _setHere = false;

    [SerializeField] private ShipController _ship;

    // private float _timer = 0f;
    // private Vector3 _targetPosition;

    private void Update()
    {
        if (_setHere)
        {
            _ship.SetDestination(transform.position);
            _setHere = false;
        }
        // _timer -= Time.deltaTime;

        // if (_timer <= 0f)
        // {
        //     _ship.SetDestination(transform.position);
        //     _timer = _cooldown;
        //     _targetPosition = Random.insideUnitCircle.XZ() * _distance + transform.position;
        // }

        // transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
    }
}
