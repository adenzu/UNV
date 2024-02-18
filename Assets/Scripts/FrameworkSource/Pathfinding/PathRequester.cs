using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Pathfinding;

public class PathRequester : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] private GridManager gridManager;
=======
    [SerializeField] private TilingManager tilingManager;
>>>>>>> Stashed changes
    [SerializeField] private PathRequestManager pathRequestManager;

    public Transform seeker, target;
    [Range(2, 20)] public float angleChangeOverDistance;
    private Vector3[] pathWithRotationalCost, pathWithNoRotationalCost;

    private void Update()
    {
        if (!pathRequestManager.IsProcessingPath)
        {
<<<<<<< Updated upstream
            pathRequestManager.RequestPath(seeker.position, target.position, angleChangeOverDistance, (path, success) => { if (success) pathWithRotationalCost = path; }, () => seeker.transform.forward.XZ());
            pathRequestManager.RequestPath(seeker.position, target.position, 360, (path, success) => { if (success) pathWithNoRotationalCost = path; }, () => seeker.transform.forward.XZ());
=======
            pathRequestManager.RequestPath(
                seeker.position,
                target.position,
                (path, success) => { if (success) pathWithRotationalCost = path; },
                angleChangeOverDistance,
                new AStarWithRotation.GetDefaultDirectionDelegate(() => seeker.transform.forward.XZ())
            );
            pathRequestManager.RequestPath(
                seeker.position,
                target.position,
                (path, success) => { if (success) pathWithNoRotationalCost = path; },
                360f,
                new AStarWithRotation.GetDefaultDirectionDelegate(() => seeker.transform.forward.XZ())
            );
>>>>>>> Stashed changes
        }
    }

    private void OnDrawGizmos()
    {
        if (pathWithRotationalCost != null)
        {
            for (int i = 0; i < pathWithRotationalCost.Length; i++)
            {
                Gizmos.color = Color.green;
<<<<<<< Updated upstream
                Gizmos.DrawCube(pathWithRotationalCost[i], gridManager.NodeSize * Vector3.one);
=======
                Gizmos.DrawCube(pathWithRotationalCost[i], tilingManager.NodeSize * Vector3.one);
>>>>>>> Stashed changes
            }
        }
        if (pathWithNoRotationalCost != null)
        {
            for (int i = 0; i < pathWithNoRotationalCost.Length; i++)
            {
                Gizmos.color = Color.blue;
<<<<<<< Updated upstream
                Gizmos.DrawCube(pathWithNoRotationalCost[i], gridManager.NodeSize * Vector3.one);
=======
                Gizmos.DrawCube(pathWithNoRotationalCost[i], tilingManager.NodeSize * Vector3.one);
>>>>>>> Stashed changes
            }
        }
    }
}
