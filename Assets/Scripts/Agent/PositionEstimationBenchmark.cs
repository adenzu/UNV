using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionEstimationBenchmark : MonoBehaviour
{
    [SerializeField] private float _averageError;
    [SerializeField] private float _averageErrorRelativeToRadius;
    [SerializeField] private MovingObstacleDataSignalEmitter[] _movingObstacleDataSignalEmitters;
    [SerializeField] private MovingObstaclePositionEstimator _movingObstaclePositionEstimator;
    [SerializeField] private float _interval = 1f;

    private float _timer = 0f;

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            _averageError = CalculateAverageError();
            _averageErrorRelativeToRadius = CalculateAverageErrorRelativeToRadius();
            _timer = _interval;
        }
    }

    public float CalculateAverageError()
    {
        float sum = 0f;
        int count = 0;
        foreach (MovingObstacleDataSignalEmitter emitter in _movingObstacleDataSignalEmitters)
        {
            MovingObstacleData data = _movingObstaclePositionEstimator.GetEstimatedData(emitter.Id);
            sum += Vector3.Distance(emitter.transform.position, data.position);
            count++;
        }
        return sum / count;
    }

    public float CalculateAverageErrorRelativeToRadius()
    {
        float sum = 0f;
        int count = 0;
        foreach (MovingObstacleDataSignalEmitter emitter in _movingObstacleDataSignalEmitters)
        {
            MovingObstacleData data = _movingObstaclePositionEstimator.GetEstimatedData(emitter.Id);
            sum += Vector3.Distance(emitter.transform.position, data.position) / emitter.Radius;
            count++;
        }
        return sum / count;
    }
}
