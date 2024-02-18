using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public interface ITiling
    {
        public int Width { get; }
        public int Height { get; }
        public float NodeSize { get; }
        public int NodeCount { get; }

        public void GenerateTiling(Vector3 origin);

        public void SetObstacleMask(LayerMask obstacleMask);

        public void SetTilingWorldSize(Vector2 tilingWorldSize);

        public void SetNodeSize(float nodeSize);

        public List<NodeBase> GetNeighbours(NodeBase pathfindingNode);

        public NodeBase GetAt(Vector3 worldPosition);

        public Vector2 GetDirection(NodeBase from, NodeBase to);

        public void DilateObstacles(int radius);

        public void EdgeObstacles(int radius);

        public void SetShow(bool show);

<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Pathfinding/IGrid.cs
        public void Clear();

        public bool IsClearPath(Vector3 from, Vector3 to);

=======
<<<<<<<< Updated upstream:Assets/Scripts/Pathfinding/IGrid.cs
========
        public bool IsClearPath(Vector3 from, Vector3 to);

>>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/ITiling.cs
>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/IGrid.cs
        public IEnumerable<NodeBase> GetNodes();
    }
}