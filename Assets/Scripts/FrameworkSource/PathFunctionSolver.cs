using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFunctionSolver
{
    public Func<float, Vector3> ComputePathFunction(MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence);
    public Func<float, Vector3> ComputePathFunction(MovingObstaclePositionEstimator.MovingObstacleDataSequence dataSequence, out Vector3 absoluteError);
}
