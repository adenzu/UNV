using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Pathfinding;

public class PathRequestManager : MonoBehaviour
{
    [SerializeField] private Pathfinding pathfinding;

    public bool IsProcessingPath => isProcessingPath;
    private bool isProcessingPath;

    private readonly Queue<PathRequest> pathRequestQueue = new();
    private PathRequest currentPathRequest;

    public void RequestPath(
        Vector3 pathStart,
        Vector3 pathEnd,
        float angleChange,
        System.Action<Vector3[], bool> callback,
        Pathfinding.GetDefaultDirectionDelegate getDefaultDirection
    )
    {
        PathRequest newRequest = new(pathStart, pathEnd, angleChange, callback, getDefaultDirection);
        pathRequestQueue.Enqueue(newRequest);
        TryProcessNext();
    }

    public void OnPathProcessFinish(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.GetDefaultDirection = currentPathRequest.getDefaultDirection;
            pathfinding.angleChangeOverDistance = currentPathRequest.angleChange;
            pathfinding.StartFindingPath(currentPathRequest.start, currentPathRequest.end);
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
