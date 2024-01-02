using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleDataSignalEmitter : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private MovingObstacleDataSignalReceiver _movingObstacleDataSignalReceiver;
    [SerializeField] private float _interval = 1f;

    public int Id => transform.GetInstanceID();
    public float Radius => _radius;

    private float _timer = 0f;

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            SendData();
            _timer = _interval;
        }
    }

    public void SendData()
    {
        if (_movingObstacleDataSignalReceiver == null)
        {
            return;
        }
        _movingObstacleDataSignalReceiver.ReceiveData(
            Id,
            new MovingObstacleData
            {
                position = transform.position,
                radius = _radius,
            }
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
