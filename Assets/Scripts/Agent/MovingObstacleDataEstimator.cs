using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleDataEstimator : MonoBehaviour
{
    [SerializeField] private float _maxAngularSpeed = 1f;
    [SerializeField] private MovingObstacleDataSignalReceiver _movingObstacleDataSignalReceiver;

    private Dictionary<int, MovingObstacleDataWithTimestamp> _data = new();
    private Dictionary<int, Vector3> _estimatedVelocities = new();
    private Dictionary<int, MovingObstacleData> _estimatedData = new();

    private void Start()
    {
        _movingObstacleDataSignalReceiver.OnDataReceived += OnDataReceived;
    }

    private void Update()
    {
        foreach (int id in _data.Keys)
        {
            UpdateEstimation(id, Time.time);
        }
    }

    public MovingObstacleData GetEstimatedData(int id)
    {
        _estimatedData.TryGetValue(id, out MovingObstacleData data);
        return data;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (MovingObstacleData data in _estimatedData.Values)
        {
            Gizmos.DrawSphere(data.position, data.radius * 0.5f);
        }
        // Gizmos.color = Color.grey;
        // foreach (MovingObstacleDataWithTimestamp data in _data.Values)
        // {
        //     Gizmos.DrawSphere(data.currentMovingObstacleData.position, data.currentMovingObstacleData.radius);
        // }
        // Gizmos.color = Color.blue;
        // foreach (MovingObstacleDataWithTimestamp data in _data.Values)
        // {
        //     Vector3 position = data.currentMovingObstacleData.position;
        //     Vector3 direction = (position - data.previousMovingObstacleData.position).normalized;
        //     float timePassed = Time.time - data.currentDataTime;
        //     float angle = _maxAngularSpeed * timePassed;
        //     Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.up) * direction;
        //     Vector3 oppositeRotatedDirection = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
        //     float estimatedSpeed = EstimateVelocity(data).magnitude;
        //     Gizmos.DrawLine(position, position + rotatedDirection * estimatedSpeed * timePassed);
        //     Gizmos.DrawLine(position, position + oppositeRotatedDirection * estimatedSpeed * timePassed);
        // }
    }

    private Vector3 EstimateVelocity(int id)
    {
        _data.TryGetValue(id, out MovingObstacleDataWithTimestamp data);
        Vector3 positionChange = data.currentMovingObstacleData.position - data.previousMovingObstacleData.position;
        float timeChange = data.currentDataTime - data.previousDataTime;
        return positionChange / timeChange;
    }

    private Vector3 EstimateVelocity(MovingObstacleDataWithTimestamp data)
    {
        Vector3 positionChange = data.currentMovingObstacleData.position - data.previousMovingObstacleData.position;
        float timeChange = data.currentDataTime - data.previousDataTime;
        return positionChange / timeChange;
    }

    private Vector3 EstimatePosition(int id, float time)
    {
        _data.TryGetValue(id, out MovingObstacleDataWithTimestamp data);
        _estimatedVelocities.TryGetValue(id, out Vector3 velocity);

        return data.currentMovingObstacleData.position + velocity * (time - data.currentDataTime);
    }

    private void OnDataReceived(int id, MovingObstacleData data)
    {
        UpdateData(id, data);
    }

    private void UpdateData(int id, MovingObstacleData data)
    {
        if (!_data.ContainsKey(id))
        {
            _data.Add(id, new MovingObstacleDataWithTimestamp
            {
                currentMovingObstacleData = data,
                currentDataTime = Time.time,
            });
            _estimatedVelocities.Add(id, Vector3.zero);
        }
        else
        {
            _data.TryGetValue(id, out MovingObstacleDataWithTimestamp updateData);

            updateData.previousMovingObstacleData = updateData.currentMovingObstacleData;
            updateData.previousDataTime = updateData.currentDataTime;

            updateData.currentMovingObstacleData = data;
            updateData.currentDataTime = Time.time;

            _data[id] = updateData;
            _estimatedVelocities[id] = EstimateVelocity(id);
        }
    }

    private void UpdateEstimation(int id, float time)
    {
        if (!_estimatedData.ContainsKey(id))
        {
            _data.TryGetValue(id, out MovingObstacleDataWithTimestamp data);
            _estimatedData.Add(id, new MovingObstacleData
            {
                position = data.currentMovingObstacleData.position,
                radius = data.currentMovingObstacleData.radius,
            });
        }
        else
        {
            _estimatedData.TryGetValue(id, out MovingObstacleData updatedData);
            updatedData.position = EstimatePosition(id, time);
            _estimatedData[id] = updatedData;
        }
    }

    private struct MovingObstacleDataWithTimestamp
    {
        public MovingObstacleData currentMovingObstacleData, previousMovingObstacleData;
        public float currentDataTime, previousDataTime;
    }
}
