using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    public static PathRequestManager Instance { get; private set; }

    private Queue<PathRequest> _pathRequestQueue;
    private PathRequest _currentPathRequest;
    private bool _isProcessingPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _pathRequestQueue = new Queue<PathRequest>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void RequestPath(
        Vector3 pathStart, 
        Vector3 pathEnd, 
        System.Action<Vector3[], bool> callback, 
        Pathfinding.GetDefaultDirectionDelegate getDefaultDirection,
        Pathfinding.AngleCostFunctionDelegate angleCostFunction,
        int neighbourRadius
    )
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback, getDefaultDirection, angleCostFunction, neighbourRadius);
        Instance._pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    public static void OnPathProcessFinish(Vector3[] path, bool success)
    {
        Instance._currentPathRequest.callback(path, success);
        Instance._isProcessingPath = false;
        Instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!_isProcessingPath && _pathRequestQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessingPath = true;
            Pathfinding.GetDefaultDirection = _currentPathRequest.getDefaultDirection;
            Pathfinding.AngleCostFunction = _currentPathRequest.angleCostFunction;
            Pathfinding.NeighbourRadius = _currentPathRequest.neighbourRadius;
            Pathfinding.StartFindingPath(_currentPathRequest.start, _currentPathRequest.end);
        }
    }

    public struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public System.Action<Vector3[], bool> callback;
        public Pathfinding.GetDefaultDirectionDelegate getDefaultDirection;
        public Pathfinding.AngleCostFunctionDelegate angleCostFunction;
        public int neighbourRadius;

        public PathRequest(
            Vector3 start, 
            Vector3 end, 
            System.Action<Vector3[], bool> callback,
            Pathfinding.GetDefaultDirectionDelegate getDefaultDirection,
            Pathfinding.AngleCostFunctionDelegate angleCostFunction,
            int neighbourRadius
        )
        {
            this.start = start;
            this.end = end;
            this.callback = callback;
            this.getDefaultDirection = getDefaultDirection;
            this.angleCostFunction = angleCostFunction;
            this.neighbourRadius = neighbourRadius;
        }
    }
}
