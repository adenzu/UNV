using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePathFunctionSolver : IPathFunctionSolver
{
    public Func<float, Vector3> ComputePathFunction(MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence)
    {
        var currentData = dataSequence.GetMostRecentData();
        var previousData = dataSequence.GetPreviousData();
        Vector3 positionChange = currentData.data.position - previousData.data.position;
        float timeChange = currentData.time - previousData.time;
        return t => currentData.data.position + (positionChange / timeChange) * (t - currentData.time);
    }

    public Func<float, Vector3> ComputePathFunction(MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence, out Vector3 absoluteError)
    {
        var currentData = dataSequence.GetMostRecentData();
        var previousData = dataSequence.GetPreviousData();
        Vector3 positionChange = currentData.data.position - previousData.data.position;
        float timeChange = currentData.time - previousData.time;
        Vector3 pathFunction(float t) => currentData.data.position + (positionChange / timeChange) * (t - currentData.time);
        absoluteError = Vector3.one * PathObjectiveFunction(pathFunction, dataSequence);
        return pathFunction;
    }


    private float PathObjectiveFunction(Func<float, Vector3> pathFunction, MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence)
    {
        float cost = 0;
        if (dataSequence == null)
        {
            return cost;
        }
        for (int i = 0; i < dataSequence.Length; i++)
        {
            var data = dataSequence.GetData(i);
            Vector3 predictedPosition = pathFunction(data.time);
            cost += Vector3.Distance(data.data.position, predictedPosition);
        }
        return cost;
    }
}
