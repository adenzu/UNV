using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;

public class MovingObstaclePositionEstimator : MonoBehaviour
{
    [SerializeField] private MovingObstacleDataSignalReceiver _movingObstacleDataSignalReceiver;
    [SerializeField] private int _dataSequenceLength = 3;

    private Dictionary<int, MovingObstacleDataSequence> _data = new();
    private Dictionary<int, MovingObstacleData> _estimatedData = new();
    private Dictionary<int, Func<float, Vector3>> _estimatedPathFunctions = new();
    private Dictionary<int, float> _estimatedPathFunctionErrors = new();

    private double[] _initialGuess = new double[] { 0, 0, 0 };
    private double[] _lowerBound = new double[] { 0, -50, -Mathf.PI };
    private double[] _upperBound = new double[] { 100, 50, Mathf.PI };

    private void Start()
    {
        _movingObstacleDataSignalReceiver.OnDataReceived += OnDataReceived;
    }

    private void Update()
    {
        foreach (int id in _data.Keys)
        {
            UpdateEstimatedData(id, Time.time);
        }
    }

    public MovingObstacleData GetEstimatedData(int id)
    {
        _estimatedData.TryGetValue(id, out MovingObstacleData data);
        return data;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        foreach (MovingObstacleDataSequence data in _data.Values)
        {
            foreach (MovingObstacleDataWithTime currentData in data.data)
            {
                Gizmos.DrawSphere(currentData.data.position, currentData.data.radius * 0.5f);
            }
        }

        Gizmos.color = Color.green;
        foreach (MovingObstacleData data in _estimatedData.Values)
        {
            Gizmos.DrawWireSphere(data.position, data.radius);
        }
    }

    private Vector3 EstimatePosition(int id, float time)
    {
        if (!_estimatedPathFunctions.ContainsKey(id))
        {
            if (!_data.ContainsKey(id))
            {
                return Vector3.zero;
            }
            return _data[id].GetMostRecentData().data.position;
        }
        return _estimatedPathFunctions[id](time);
    }

    private void OnDataReceived(int id, MovingObstacleData data)
    {
        UpdateData(id, data);
    }

    private void UpdateData(int id, MovingObstacleData data)
    {
        if (!_data.ContainsKey(id))
        {
            _data.Add(id, new MovingObstacleDataSequence(_dataSequenceLength));
        }
        _data[id].Add(data, Time.time);
        UpdatePathFunction(id);
    }

    private void UpdatePathFunction(int id)
    {
        _data.TryGetValue(id, out MovingObstacleDataSequence dataSequence);
        if (dataSequence.Length < 3)
        {
            _estimatedPathFunctions[id] = t => SimplePathFunction(t, dataSequence);
            _estimatedPathFunctionErrors[id] = (float)PathObjectiveFunction(_estimatedPathFunctions[id], dataSequence);
            return;
        }
        var optimizer = new BfgsBMinimizer(1e-5, 1e-5, 1e-5);
        // var lowerBound = new DenseVector(_lowerBound);
        // var upperBound = new DenseVector(_upperBound);
        var initialGuess = new DenseVector(_initialGuess);
        var objective = ObjectiveFunction.Value(x => PathObjectiveFunction(x, dataSequence));
        // var objectiveWithGradient = new MathNet.Numerics.Optimization.ObjectiveFunctions.ForwardDifferenceGradientObjectiveFunction(objective, lowerBound, upperBound);
        // var result = optimizer.FindMinimum(objectiveWithGradient, lowerBound, upperBound, initialGuess);
        var result = NelderMeadSimplex.Minimum(objective, initialGuess);
        var newPathFunction = GetPathFunction(result.MinimizingPoint.ToArray(), dataSequence.GetOldestData());

        if (!_estimatedPathFunctions.ContainsKey(id))
        {
            _estimatedPathFunctions.Add(id, newPathFunction);
            _estimatedPathFunctionErrors.Add(id, (float)result.FunctionInfoAtMinimum.Value);
        }
        else if (PathObjectiveFunction(result.MinimizingPoint, dataSequence) < PathObjectiveFunction(_estimatedPathFunctions[id], dataSequence))
        {
            _estimatedPathFunctions[id] = newPathFunction;
            _estimatedPathFunctionErrors[id] = (float)result.FunctionInfoAtMinimum.Value;
        }
    }

    private Vector3 SimplePathFunction(float t, MovingObstacleDataSequence dataSequence)
    {
        MovingObstacleDataWithTime currentData = dataSequence.GetMostRecentData();
        MovingObstacleDataWithTime previousData = dataSequence.GetPreviousData();
        Vector3 positionChange = currentData.data.position - previousData.data.position;
        float timeChange = currentData.time - previousData.time;
        return currentData.data.position + (positionChange / timeChange) * (t - currentData.time);
    }

    private void UpdateEstimatedData(int id, float time)
    {
        if (!_estimatedData.ContainsKey(id))
        {
            _data.TryGetValue(id, out MovingObstacleDataSequence dataSequence);
            MovingObstacleDataWithTime currentData = dataSequence.GetMostRecentData();
            _estimatedData.Add(id, new MovingObstacleData
            {
                position = currentData.data.position,
                radius = currentData.data.radius,
            });
        }
        else
        {
            MovingObstacleData currentData = _data[id].GetMostRecentData().data;
            _estimatedData.TryGetValue(id, out MovingObstacleData updatedData);
            updatedData.position = EstimatePosition(id, time);
            updatedData.radius = _estimatedPathFunctionErrors[id] + currentData.radius;
            _estimatedData[id] = updatedData;
        }
    }

    /// <summary>
    /// The cost function of the prediction of parameters of the path that is taken by the predicted moving obstacle.
    /// </summary>
    /// <param name="x">Array of three float inputs for the speed, angular speed, and initial angle in that order.</param>
    /// <returns>How off is the predicted path function.</returns> 
    private double PathObjectiveFunction(Vector<double> x, MovingObstacleDataSequence dataSequence)
    {
        double cost = 0;
        if (dataSequence == null)
        {
            return cost;
        }
        Func<float, Vector3> pathFunction = GetPathFunction(x.ToArray(), dataSequence.GetOldestData());
        for (int i = 0; i < dataSequence.Length; i++)
        {
            MovingObstacleDataWithTime data = dataSequence.GetData(i);
            Vector3 predictedPosition = pathFunction(data.time);
            cost += Vector3.Distance(data.data.position, predictedPosition);
        }
        return cost;
    }

    private double PathObjectiveFunction(Func<float, Vector3> pathFunction, MovingObstacleDataSequence dataSequence)
    {
        double cost = 0;
        if (dataSequence == null)
        {
            return cost;
        }
        for (int i = 0; i < dataSequence.Length; i++)
        {
            MovingObstacleDataWithTime data = dataSequence.GetData(i);
            Vector3 predictedPosition = pathFunction(data.time);
            cost += Vector3.Distance(data.data.position, predictedPosition);
        }
        return cost;
    }

    /// The path function assumes constant speed and angular speed. for XZ plane
    /// angle(t) = angularSpeed * t + initialAngle
    /// velocity(t) = speed * (cos(angle(t)), sin(angle(t)))
    /// position(t) = initialPosition + integral(velocity(t)) 
    ///             = P0 + (v / w) * (sin(w0) - sin(w * t + w0), cos(w * t + w0) - cos(w0))
    /// assume position(0) = P0 = (0, 0)
    private Func<float, Vector3> GetPathFunction(double[] x, MovingObstacleDataWithTime relativeTo)
    {
        float speed = (float)x[0];
        float angularSpeed = (float)x[1];
        float initialAngle = (float)x[2];

        Vector3 relativePosition = relativeTo.data.position;
        float relativeTime = relativeTo.time;

        return (float t) =>
        {
            Vector3 position;
            t -= relativeTime;
            if (angularSpeed == 0)
            {
                Vector3 direction = new Vector3(
                    Mathf.Cos(initialAngle),
                    0,
                    Mathf.Sin(initialAngle)
                );
                position = speed * t * direction;
            }
            else
            {
                Vector3 direction = new Vector3(
                    Mathf.Sin(initialAngle) - Mathf.Sin(initialAngle + angularSpeed * t),
                    0,
                    Mathf.Cos(initialAngle + angularSpeed * t) - Mathf.Cos(initialAngle)
                );
                position = -(speed / angularSpeed) * direction;
            }

            return position + relativePosition;
        };
    }

    private class MovingObstacleDataSequence
    {
        public MovingObstacleDataWithTime[] data;
        public int Length { get; private set; }

        public MovingObstacleDataSequence(int length)
        {
            data = new MovingObstacleDataWithTime[length];
            Length = 0;
        }

        public void Add(MovingObstacleData data, float time)
        {
            for (int i = this.data.Length - 1; i > 0; i--)
            {
                this.data[i] = this.data[i - 1];
            }
            this.data[0] = new MovingObstacleDataWithTime
            {
                data = data,
                time = time,
            };
            Length++;
            if (Length > this.data.Length)
            {
                Length = this.data.Length;
            }
        }

        public MovingObstacleDataWithTime GetMostRecentData()
        {
            return data[0];
        }

        public MovingObstacleDataWithTime GetPreviousData()
        {
            return data[1];
        }

        public MovingObstacleDataWithTime GetOldestData()
        {
            return data[data.Length - 1];
        }

        public MovingObstacleDataWithTime GetData(int index)
        {
            return data[index];
        }
    }

    private struct MovingObstacleDataWithTime
    {
        public MovingObstacleData data;
        public float time;
    }
}
