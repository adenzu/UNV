using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Pathfinding;

public class PathRequester : MonoBehaviour
{
    public Transform seeker, target;
    [Range(2, 20)] public float angleChangeOverDistance;
    private Vector3[] _path, _path1;

    private void Update()
    {
        if (!PathRequestManager.IsProcessingPath)
        {
            PathRequestManager.RequestPath(seeker.position, target.position, angleChangeOverDistance, OnPathFound, () => seeker.transform.forward.XZ());
            PathRequestManager.RequestPath(seeker.position, target.position, 360, OnPathFound1, () => seeker.transform.forward.XZ());
        }
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        if (success)
        {
            _path = path;
        }
    }

    private void OnPathFound1(Vector3[] path, bool success)
    {
        if (success)
        {
            _path1 = path;
        }
    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            for (int i = 0; i < _path.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(_path[i], GridManager.Instance.NodeSize * Vector3.one);
            }
        }
        if (_path1 != null)
        {
            for (int i = 0; i < _path1.Length; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(_path1[i], GridManager.Instance.NodeSize * Vector3.one);
            }
        }
    }
}
