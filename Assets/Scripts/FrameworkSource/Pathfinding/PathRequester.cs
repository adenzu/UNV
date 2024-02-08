using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Pathfinding;

public class PathRequester : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathRequestManager pathRequestManager;

    public Transform seeker, target;
    [Range(2, 20)] public float angleChangeOverDistance;
    private Vector3[] pathWithRotationalCost, pathWithNoRotationalCost;

    private void Update()
    {
        if (!pathRequestManager.IsProcessingPath)
        {
            pathRequestManager.RequestPath(seeker.position, target.position, angleChangeOverDistance, (path, success) => { if (success) pathWithRotationalCost = path; }, () => seeker.transform.forward.XZ());
            pathRequestManager.RequestPath(seeker.position, target.position, 360, (path, success) => { if (success) pathWithNoRotationalCost = path; }, () => seeker.transform.forward.XZ());
        }
    }

    private void OnDrawGizmos()
    {
        if (pathWithRotationalCost != null)
        {
            for (int i = 0; i < pathWithRotationalCost.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(pathWithRotationalCost[i], gridManager.NodeSize * Vector3.one);
            }
        }
        if (pathWithNoRotationalCost != null)
        {
            for (int i = 0; i < pathWithNoRotationalCost.Length; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(pathWithNoRotationalCost[i], gridManager.NodeSize * Vector3.one);
            }
        }
    }
}
