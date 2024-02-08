using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;

public class NelderMeadPathFunctionSolver : IPathFunctionSolver
{
    private readonly double[] initialGuess = new double[] { 0, 0, 0 };
    private readonly double[] lowerBound = new double[] { 0, -50, -Mathf.PI };
    private readonly double[] upperBound = new double[] { 100, 50, Mathf.PI };

    private const int RETRY_COUNT = 3;

    public Func<float, Vector3> ComputePathFunction(MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence)
    {
        return ComputePathFunction(dataSequence, out _);
    }

    public Func<float, Vector3> ComputePathFunction(MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence, out Vector3 absoluteError)
    {
        var initialGuess = new DenseVector(this.initialGuess);
        var objective = ObjectiveFunction.Value(x => PathObjectiveFunction(x, dataSequence));
        var result = NelderMeadSimplex.Minimum(objective, initialGuess);

        for (int i = RETRY_COUNT - 1; i > 0; i--)
        {
            if (!IsWithinBounds(result.MinimizingPoint.ToArray()))
            {
                result = NelderMeadSimplex.Minimum(objective, initialGuess);
            }
            else
            {
                break;
            }
        }

        absoluteError = Vector3.one * (float)result.FunctionInfoAtMinimum.Value;

        Func<float, Vector3> pathFunction = GetPathFunction(result.MinimizingPoint.ToArray(), dataSequence.GetOldestData());
        return pathFunction;
    }

    private bool IsWithinBounds(double[] x)
    {
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] < lowerBound[i] || x[i] > upperBound[i])
            {
                return false;
            }
        }
        return true;
    }

    private double PathObjectiveFunction(Vector<double> x, MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence)
    {
        double cost = 0;
        if (dataSequence == null)
        {
            return cost;
        }
        Func<float, Vector3> pathFunction = GetPathFunction(x.ToArray(), dataSequence.GetOldestData());
        for (int i = 0; i < dataSequence.Length; i++)
        {
            MovingObstaclePositionEstimator.MovingObstacleDataWithTime data = dataSequence.GetData(i);
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
    private Func<float, Vector3> GetPathFunction(double[] x, MovingObstaclePositionEstimator.MovingObstacleDataWithTime relativeTo)
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
                    Mathf.Sin(initialAngle + angularSpeed * t) - Mathf.Sin(initialAngle),
                    0,
                    Mathf.Cos(initialAngle) - Mathf.Cos(initialAngle + angularSpeed * t)
                );
                position = (speed / angularSpeed) * direction;
            }

            return position + relativePosition;
        };
    }
}
