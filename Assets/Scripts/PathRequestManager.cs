using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Pathfinding;

public class PathRequestManager : MonoBehaviour
{
    public static PathRequestManager Instance { get; private set; }

    public static bool IsProcessingPath => Instance._isProcessingPath;

    private Queue<PathRequest> _pathRequestQueue;
    private PathRequest _currentPathRequest;

    private bool _isProcessingPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _pathRequestQueue = new Queue<PathRequest>();
    }

    public static void RequestPath(
        Vector3 pathStart,
        Vector3 pathEnd,
        float angleChange,
        System.Action<Vector3[], bool> callback,
        Pathfinding.GetDefaultDirectionDelegate getDefaultDirection
    )
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, angleChange, callback, getDefaultDirection);
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
            Pathfinding.angleChangeOverDistance = _currentPathRequest.angleChange;
            Pathfinding.StartFindingPath(_currentPathRequest.start, _currentPathRequest.end);
        }
    }

    public struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public float angleChange;
        public System.Action<Vector3[], bool> callback;
        public Pathfinding.GetDefaultDirectionDelegate getDefaultDirection;

        public PathRequest(
            Vector3 start,
            Vector3 end,
            float angleChange,
            System.Action<Vector3[], bool> callback,
            Pathfinding.GetDefaultDirectionDelegate getDefaultDirection
        )
        {
            this.start = start;
            this.end = end;
            this.angleChange = angleChange;
            this.callback = callback;
            this.getDefaultDirection = getDefaultDirection;
        }
    }
}
