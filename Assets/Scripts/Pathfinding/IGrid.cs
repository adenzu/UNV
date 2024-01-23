using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public interface IGrid
    {
        public int Width { get; }
        public int Height { get; }
        public float NodeSize { get; }
        public int NodeCount { get; }

        public void GenerateGrid(Vector3 origin);

        public void SetObstacleMask(LayerMask obstacleMask);

        public void SetGridWorldSize(Vector2 gridWorldSize);

        public void SetNodeSize(float nodeSize);

        public List<NodeBase> GetNeighbours(NodeBase pathfindingNode);

        public NodeBase GetAt(Vector3 worldPosition);

        public Vector2 GetDirection(NodeBase from, NodeBase to);

        public void DilateObstacles(int radius);

        public void EdgeObstacles(int radius);

        public void SetShow(bool show);

        public void Clear();

        public bool IsClearPath(Vector3 from, Vector3 to);

        public IEnumerable<NodeBase> GetNodes();
    }
}