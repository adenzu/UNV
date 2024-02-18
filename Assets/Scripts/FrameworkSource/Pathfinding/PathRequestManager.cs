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
<<<<<<< Updated upstream
        Vector3 pathStart,
        Vector3 pathEnd,
        float angleChange,
        System.Action<Vector3[], bool> callback,
        Pathfinding.GetDefaultDirectionDelegate getDefaultDirection
    )
    {
        PathRequest newRequest = new(pathStart, pathEnd, angleChange, callback, getDefaultDirection);
=======
        Vector3 start,
        Vector3 end,
        System.Action<Vector3[], bool> callback,
        params object[] parameters
    )
    {
        PathRequest newRequest = new(start, end, callback, parameters);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.GetDefaultDirection = currentPathRequest.getDefaultDirection;
            pathfinding.angleChangeOverDistance = currentPathRequest.angleChange;
=======
        if (!pathfinding.InitializedPathfindingAlgorithm)
        {
            Invoke(nameof(TryProcessNext), 0.1f);
            return;
        }
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            isProcessingPath = true;
            currentPathRequest = pathRequestQueue.Dequeue();
            pathfinding.SetParameters(currentPathRequest.parameters);
>>>>>>> Stashed changes
            pathfinding.StartFindingPath(currentPathRequest.start, currentPathRequest.end);
        }
    }

    public struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
<<<<<<< Updated upstream
        public float angleChange;
        public System.Action<Vector3[], bool> callback;
        public Pathfinding.GetDefaultDirectionDelegate getDefaultDirection;
=======
        public System.Action<Vector3[], bool> callback;
        public object[] parameters;
>>>>>>> Stashed changes

        public PathRequest(
            Vector3 start,
            Vector3 end,
<<<<<<< Updated upstream
            float angleChange,
            System.Action<Vector3[], bool> callback,
            Pathfinding.GetDefaultDirectionDelegate getDefaultDirection
=======
            System.Action<Vector3[], bool> callback,
            params object[] parameters
>>>>>>> Stashed changes
        )
        {
            this.start = start;
            this.end = end;
<<<<<<< Updated upstream
            this.angleChange = angleChange;
            this.callback = callback;
            this.getDefaultDirection = getDefaultDirection;
=======
            this.callback = callback;
            this.parameters = parameters;
>>>>>>> Stashed changes
        }
    }
}
