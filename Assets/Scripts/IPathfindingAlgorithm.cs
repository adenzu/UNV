using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UNV.Pathfinding
{
    public interface IPathfindingAlgorithm
    {
        public void SetParameters(params object[] parameters);
        public void SetTiling(ITiling tiling);
        public Vector3[] FindPath(Vector3 start, Vector3 target);
    }
}
