using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using UnityEditor;
using System.Linq;
using Unity.VisualScripting;

[RequireComponent(typeof(MovingObstacleDataSignalReceiver))]
public class MovingObstaclePositionEstimator : MonoBehaviour
{
    [SerializeField] private MonoScript pathFunctionSolverScript;
    [SerializeField, Min(1)] private int savedDataSequenceLength = 3;
    [SerializeField, Min(0)] private int useSimplePathFunctionWhenDataLessThan = 3;
    [SerializeField] private bool showGizmos = false;

    public PositionDirectionRadius[] MovingObstacles => GetEstimated();

    private MovingObstacleDataSignalReceiver movingObstacleDataSignalReceiver;

    private readonly Dictionary<int, MovingObstacleDataSequence> data = new();
    private readonly Dictionary<int, MovingObstacleData> estimatedData = new();
    private readonly Dictionary<int, PositionDirectionRadius> estimated = new();
    private readonly Dictionary<int, Func<float, Vector3>> estimatedPathFunctions = new();
    private readonly Dictionary<int, float> estimatedPathFunctionErrors = new();

    private IPathFunctionSolver pathFunctionSolver;
    private readonly SimplePathFunctionSolver simplePathFunctionSolver = new();

    private void OnValidate()
    {
        if (!Util.HandleMonoScriptFieldClassInheritance(pathFunctionSolverScript, typeof(IPathFunctionSolver), "Path Function Solver"))
        {
            pathFunctionSolverScript = null;
        }
    }

    private void Awake()
    {
        if (pathFunctionSolverScript != null)
        {
<<<<<<< Updated upstream
            pathFunctionSolver = (IPathFunctionSolver)pathFunctionSolverScript.GetClass().GetConstructor(new Type[] { }).Invoke(new object[] { });
=======
            pathFunctionSolver = Util.InstantiateMonoScriptObject<IPathFunctionSolver>(pathFunctionSolverScript);
>>>>>>> Stashed changes
        }
        movingObstacleDataSignalReceiver = GetComponent<MovingObstacleDataSignalReceiver>();
    }

    private void Start()
    {
        movingObstacleDataSignalReceiver.OnDataReceived += OnDataReceived;
    }

    private void Update()
    {
        foreach (int id in data.Keys)
        {
            UpdateEstimatedData(id, Time.time);
<<<<<<< Updated upstream
=======
            Debug.Log(id);
>>>>>>> Stashed changes
        }
    }

    public PositionDirectionRadius[] GetEstimatedWithout(Transform transform)
    {
        List<PositionDirectionRadius> estimated = new List<PositionDirectionRadius>(this.estimated.Values);
        if (this.estimated.ContainsKey(transform.GetInstanceID()))
        {
            estimated.Remove(this.estimated[transform.GetInstanceID()]);
        }
        return estimated.ToArray();
    }

<<<<<<< Updated upstream
=======
    public PositionDirectionRadius[] GetEstimatedWithout(int id)
    {
        List<PositionDirectionRadius> estimated = new List<PositionDirectionRadius>(this.estimated.Values);
        if (this.estimated.ContainsKey(id))
        {
            estimated.Remove(this.estimated[id]);
        }
        return estimated.ToArray();
    }

>>>>>>> Stashed changes
    public PositionDirectionRadius[] GetEstimated()
    {
        return new List<PositionDirectionRadius>(estimated.Values).ToArray();
    }

    public PositionDirectionRadius GetEstimated(int id)
    {
        return estimated[id];
    }

    public PositionDirectionRadius GetEstimated(Transform transform)
    {
        return estimated[transform.GetInstanceID()];
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
        {
            return;
        }

        Gizmos.color = Color.gray;
        foreach (MovingObstacleDataSequence data in data.Values)
        {
            foreach (MovingObstacleDataWithTime currentData in data.data)
            {
                Gizmos.DrawSphere(currentData.data.position, currentData.data.radius * 0.5f);
            }
        }

        Gizmos.color = Color.green;
        foreach (MovingObstacleData data in estimatedData.Values)
        {
            Gizmos.DrawWireSphere(data.position, data.radius);
        }
    }

    private Vector3 EstimatePosition(int id, float time)
    {
        if (!estimatedPathFunctions.ContainsKey(id))
        {
            if (!data.ContainsKey(id))
            {
                return Vector3.zero;
            }
            return data[id].GetMostRecentData().data.position;
        }
        return estimatedPathFunctions[id](time);
    }

    private void OnDataReceived(int id, MovingObstacleData data)
    {
        UpdateData(id, data);
    }

    private void UpdateData(int id, MovingObstacleData data)
    {
        if (!this.data.ContainsKey(id))
        {
            this.data.Add(id, new MovingObstacleDataSequence(savedDataSequenceLength));
        }
        this.data[id].Add(data, Time.time);
        UpdatePathFunction(id);
    }

    private void UpdatePathFunction(int id)
    {
        data.TryGetValue(id, out MovingObstacleDataSequence dataSequence);
        Vector3 absoluteError;

        if (pathFunctionSolver == null || dataSequence.Length < useSimplePathFunctionWhenDataLessThan)
        {
            const int NUMBER_OF_NECESSARY_POSITIONS_FOR_NON_LINEAR_PATH_DERIVATION = 3;

            if (pathFunctionSolver == null || dataSequence.Length < NUMBER_OF_NECESSARY_POSITIONS_FOR_NON_LINEAR_PATH_DERIVATION)
            {
                estimatedPathFunctions[id] = simplePathFunctionSolver.ComputePathFunction(dataSequence, out absoluteError);
                estimatedPathFunctionErrors[id] = absoluteError.magnitude;
                return;
            }
        }

        Func<float, Vector3> pathFunction = pathFunctionSolver.ComputePathFunction(dataSequence, out absoluteError);

        if (!estimatedPathFunctions.ContainsKey(id))
        {
            estimatedPathFunctions.Add(id, pathFunction);
            estimatedPathFunctionErrors.Add(id, absoluteError.magnitude);
        }
        else
        {
            estimatedPathFunctions[id] = pathFunction;
            estimatedPathFunctionErrors[id] = absoluteError.magnitude;
        }
    }

    private void UpdateEstimatedData(int id, float time)
    {
        if (!estimatedData.ContainsKey(id))
        {
            data.TryGetValue(id, out MovingObstacleDataSequence dataSequence);
            MovingObstacleDataWithTime currentData = dataSequence.GetMostRecentData();
            estimatedData.Add(id, new MovingObstacleData
            {
                position = currentData.data.position,
                radius = currentData.data.radius,
            });
            estimated.Add(id, new PositionDirectionRadius
            {
                position = currentData.data.position,
                velocity = Vector3.zero,
                radius = currentData.data.radius,
            });
        }
        else
        {
            MovingObstacleData currentData = data[id].GetMostRecentData().data;
            estimatedData.TryGetValue(id, out MovingObstacleData updatedData);
            updatedData.position = EstimatePosition(id, time);
            updatedData.radius = estimatedPathFunctionErrors[id] + currentData.radius;
            estimatedData[id] = updatedData;
            float deltaTime = 0.1f;
            estimated[id] = new PositionDirectionRadius
            {
                position = updatedData.position,
                velocity = (updatedData.position - EstimatePosition(id, time - deltaTime)) / deltaTime,
                radius = updatedData.radius,
            };
        }
    }

    public class MovingObstacleDataSequence
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

    public struct MovingObstacleDataWithTime
    {
        public MovingObstacleData data;
        public float time;
    }

    public struct PositionDirectionRadius
    {
        public Vector3 position;
        public Vector3 velocity;
        public float radius;
    }
}
